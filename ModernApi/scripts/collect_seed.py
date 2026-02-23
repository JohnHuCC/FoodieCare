#!/usr/bin/env python3
import argparse
import json
import math
import os
import sys
import time
import urllib.error
import urllib.request
from datetime import datetime, timezone
from pathlib import Path


DEFAULT_KEYWORDS = [
    "restaurant",
    "cafe",
    "breakfast",
    "fast food",
    "bbq",
    "sushi",
    "ramen",
    "dessert"
]


def parse_args():
    p = argparse.ArgumentParser(description="Collect raw places via /api/places/hybrid-search")
    p.add_argument("--api-base", default="http://localhost:5186")
    p.add_argument("--cities-file", default="scripts/cities.seed.json")
    p.add_argument("--out", default="")
    p.add_argument("--radius-km", type=int, default=5)
    p.add_argument("--limit", type=int, default=40)
    p.add_argument("--grid-step-km", type=float, default=3.0)
    p.add_argument("--delay-ms", type=int, default=120)
    p.add_argument("--keywords", default=",".join(DEFAULT_KEYWORDS))
    return p.parse_args()


def read_json_file(path):
    with open(path, "r", encoding="utf-8") as f:
        return json.load(f)


def post_json(url, payload, timeout=30):
    data = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(
        url=url,
        data=data,
        headers={"Content-Type": "application/json"},
        method="POST",
    )
    with urllib.request.urlopen(req, timeout=timeout) as resp:
        return json.loads(resp.read().decode("utf-8"))


def check_api_health(api_base, timeout=5):
    health_url = api_base.rstrip("/") + "/api/health"
    req = urllib.request.Request(url=health_url, method="GET")
    with urllib.request.urlopen(req, timeout=timeout) as resp:
        if resp.status < 200 or resp.status >= 300:
            raise RuntimeError(f"health endpoint returned status {resp.status}")


def grid_points(center_lat, center_lng, city_radius_km, step_km):
    if step_km <= 0:
        return [(center_lat, center_lng)]

    points = []
    lat_step = step_km / 111.0
    lng_base = max(0.2, math.cos(math.radians(center_lat)))
    lng_step = step_km / (111.0 * lng_base)

    max_i = int(math.ceil(city_radius_km / step_km))
    for i in range(-max_i, max_i + 1):
        for j in range(-max_i, max_i + 1):
            lat = center_lat + i * lat_step
            lng = center_lng + j * lng_step
            d = haversine_km(center_lat, center_lng, lat, lng)
            if d <= city_radius_km:
                points.append((lat, lng))
    return points


def haversine_km(lat1, lng1, lat2, lng2):
    r = 6371.0
    d_lat = math.radians(lat2 - lat1)
    d_lng = math.radians(lng2 - lng1)
    a = (
        math.sin(d_lat / 2) ** 2
        + math.cos(math.radians(lat1))
        * math.cos(math.radians(lat2))
        * math.sin(d_lng / 2) ** 2
    )
    return r * (2 * math.atan2(math.sqrt(a), math.sqrt(1 - a)))


def main():
    args = parse_args()
    base_dir = Path(__file__).resolve().parents[1]
    cities_path = (base_dir / args.cities_file).resolve() if not os.path.isabs(args.cities_file) else Path(args.cities_file)
    if not cities_path.exists():
        print(f"cities file not found: {cities_path}", file=sys.stderr)
        return 1

    cities = read_json_file(cities_path)
    keywords = [x.strip() for x in args.keywords.split(",") if x.strip()]
    api_base = args.api_base.rstrip("/")
    endpoint = api_base + "/api/places/hybrid-search"
    now = datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%SZ")
    out_path = Path(args.out) if args.out else (base_dir / "data" / "raw" / f"hybrid_raw_{now}.json")
    out_path.parent.mkdir(parents=True, exist_ok=True)

    rows = []
    total_calls = 0
    failed_calls = 0

    try:
        check_api_health(api_base)
    except Exception as e:
        print(
            f"API preflight failed for {api_base}: {e}. "
            f"Start the API first, e.g. `dotnet run --urls {api_base}`.",
            file=sys.stderr,
        )
        return 2

    for city in cities:
        name = city.get("name", "unknown")
        lat = float(city["lat"])
        lng = float(city["lng"])
        city_radius = float(city.get("radius_km", 8))
        points = grid_points(lat, lng, city_radius, args.grid_step_km)
        print(f"[city] {name}: {len(points)} points")

        for (p_lat, p_lng) in points:
            for kw in keywords:
                payload = {
                    "query": kw,
                    "latitude": p_lat,
                    "longitude": p_lng,
                    "radiusKm": args.radius_km,
                    "limit": args.limit
                }
                total_calls += 1
                try:
                    data = post_json(endpoint, payload)
                    for item in data:
                        item["city"] = name
                        item["query"] = kw
                        item["pointLat"] = p_lat
                        item["pointLng"] = p_lng
                        rows.append(item)
                except urllib.error.HTTPError as e:
                    failed_calls += 1
                    print(f"  [http error] {name} {kw} {e.code}")
                except Exception as e:
                    failed_calls += 1
                    print(f"  [error] {name} {kw} {e}")
                    if isinstance(e, urllib.error.URLError):
                        reason = getattr(e, "reason", None)
                        if isinstance(reason, ConnectionRefusedError):
                            print(
                                f"connection refused at {api_base}; stopping early.",
                                file=sys.stderr,
                            )
                            return 3
                time.sleep(args.delay_ms / 1000.0)

    with open(out_path, "w", encoding="utf-8") as f:
        json.dump(rows, f, ensure_ascii=False, indent=2)

    print(f"saved: {out_path}")
    print(f"rows: {len(rows)}")
    print(f"calls: {total_calls}, failed: {failed_calls}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
