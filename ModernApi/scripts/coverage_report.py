#!/usr/bin/env python3
import argparse
import json
import math
from collections import defaultdict
from datetime import datetime, timezone
from pathlib import Path


TOP_C3 = [
    "早餐/早午餐",
    "咖啡、簡餐、下午茶",
    "日式料理",
    "亞洲料理(港、泰、印、星、馬)",
    "中式料理",
    "異國料理(歐洲、美洲)",
    "小吃",
    "速食料理",
    "烘焙、甜點、零食",
]


def parse_args():
    p = argparse.ArgumentParser(description="Coverage report for global_stores.json")
    p.add_argument("--seed", default="Data/global_stores.json")
    p.add_argument("--cities-file", default="scripts/cities.seed.json")
    p.add_argument("--out-json", default="data/reports/coverage_report.json")
    p.add_argument("--out-md", default="data/reports/coverage_report.md")
    p.add_argument("--min-1km", type=int, default=20)
    p.add_argument("--min-5km", type=int, default=80)
    p.add_argument("--min-10km", type=int, default=180)
    p.add_argument("--min-type-count", type=int, default=8)
    return p.parse_args()


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


def load_json(path):
    return json.loads(Path(path).read_text(encoding="utf-8"))


def city_coverage(city, seeds, thresholds, min_type_count):
    lat = float(city["lat"])
    lng = float(city["lng"])
    name = city["name"]
    radii = [1, 5, 10]
    within = {r: [] for r in radii}

    for s in seeds:
        d = haversine_km(lat, lng, float(s["lat"]), float(s["lng"]))
        for r in radii:
            if d <= r:
                within[r].append(s)

    type_counts = defaultdict(int)
    for s in within[10]:
        c3 = s.get("c3") or "其他美食"
        type_counts[c3] += 1

    top_types = sorted(type_counts.items(), key=lambda x: x[1], reverse=True)[:12]
    weak_types = []
    for c3 in TOP_C3:
        if type_counts.get(c3, 0) < min_type_count:
            weak_types.append({"c3": c3, "count": type_counts.get(c3, 0)})

    counts = {
        "1km": len(within[1]),
        "5km": len(within[5]),
        "10km": len(within[10]),
    }

    issues = []
    if counts["1km"] < thresholds["1km"]:
        issues.append(f"1km<{thresholds['1km']}")
    if counts["5km"] < thresholds["5km"]:
        issues.append(f"5km<{thresholds['5km']}")
    if counts["10km"] < thresholds["10km"]:
        issues.append(f"10km<{thresholds['10km']}")
    if weak_types:
        issues.append("type_coverage_low")

    return {
        "city": name,
        "center": {"lat": lat, "lng": lng},
        "counts": counts,
        "topTypes10km": [{"c3": k, "count": v} for k, v in top_types],
        "weakTypes10km": weak_types,
        "status": "ok" if not issues else "insufficient",
        "issues": issues,
    }


def write_markdown(path, report):
    lines = []
    lines.append("# Coverage Report")
    lines.append("")
    lines.append(f"- GeneratedAt: {report['generatedAt']}")
    lines.append(f"- TotalSeedRows: {report['totalSeedRows']}")
    lines.append(f"- InsufficientCities: {report['insufficientCityCount']}")
    lines.append("")
    lines.append("## City Summary")
    lines.append("")
    lines.append("| City | 1km | 5km | 10km | Status | Issues |")
    lines.append("|---|---:|---:|---:|---|---|")
    for c in report["cities"]:
        lines.append(
            f"| {c['city']} | {c['counts']['1km']} | {c['counts']['5km']} | {c['counts']['10km']} | {c['status']} | {', '.join(c['issues']) if c['issues'] else '-'} |"
        )
    lines.append("")
    lines.append("## Insufficient Details")
    lines.append("")
    for c in report["cities"]:
        if c["status"] == "ok":
            continue
        lines.append(f"### {c['city']}")
        lines.append(f"- Issues: {', '.join(c['issues'])}")
        if c["weakTypes10km"]:
            lines.append("- Weak types in 10km:")
            for t in c["weakTypes10km"]:
                lines.append(f"  - {t['c3']}: {t['count']}")
        lines.append("")

    Path(path).parent.mkdir(parents=True, exist_ok=True)
    Path(path).write_text("\n".join(lines), encoding="utf-8")


def main():
    args = parse_args()
    seeds = load_json(args.seed)
    cities = load_json(args.cities_file)
    thresholds = {"1km": args.min_1km, "5km": args.min_5km, "10km": args.min_10km}

    city_reports = [
        city_coverage(c, seeds, thresholds, args.min_type_count)
        for c in cities
    ]
    insufficient = [c for c in city_reports if c["status"] != "ok"]
    report = {
        "generatedAt": datetime.now(timezone.utc).isoformat(),
        "totalSeedRows": len(seeds),
        "thresholds": thresholds,
        "minTypeCount10km": args.min_type_count,
        "insufficientCityCount": len(insufficient),
        "cities": city_reports,
    }

    out_json = Path(args.out_json)
    out_json.parent.mkdir(parents=True, exist_ok=True)
    out_json.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    write_markdown(args.out_md, report)

    print(f"seed rows: {len(seeds)}")
    print(f"cities: {len(city_reports)}")
    print(f"insufficient cities: {len(insufficient)}")
    print(f"json: {out_json}")
    print(f"md: {args.out_md}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
