#!/usr/bin/env python3
import argparse
import json
import re
import sys
from pathlib import Path


def parse_args():
    p = argparse.ArgumentParser(description="Deduplicate raw places")
    p.add_argument("--in", dest="input_path", required=True)
    p.add_argument("--out", dest="output_path", required=True)
    p.add_argument("--allow-empty-input", action="store_true")
    return p.parse_args()


def normalize_name(name):
    if not name:
        return ""
    s = name.lower().strip()
    s = re.sub(r"\s+", " ", s)
    return s


def dedupe_key(item):
    name = normalize_name(item.get("name", ""))
    lat = round(float(item.get("latitude", 0.0)), 3)
    lng = round(float(item.get("longitude", 0.0)), 3)
    return f"{name}|{lat}|{lng}"


def score(item):
    s = 0
    for k in ("name", "address", "phone", "rating", "averagePrice"):
        if item.get(k) not in (None, "", []):
            s += 1
    s += float(item.get("rating", 0) or 0) / 10.0
    return s


def main():
    args = parse_args()
    input_path = Path(args.input_path)
    output_path = Path(args.output_path)
    output_path.parent.mkdir(parents=True, exist_ok=True)

    data = json.loads(input_path.read_text(encoding="utf-8"))
    if not data and not args.allow_empty_input:
        print(
            f"input rows: 0; refusing to write {output_path}. "
            f"Use --allow-empty-input to override.",
            file=sys.stderr,
        )
        return 2

    groups = {}
    for row in data:
        key = dedupe_key(row)
        if key not in groups:
            row["categoryHints"] = [row.get("query")] if row.get("query") else []
            groups[key] = row
            continue

        existing = groups[key]
        if score(row) > score(existing):
            keep_hints = set(existing.get("categoryHints", []))
            if row.get("query"):
                keep_hints.add(row["query"])
            row["categoryHints"] = sorted(h for h in keep_hints if h)
            groups[key] = row
        else:
            hints = set(existing.get("categoryHints", []))
            if row.get("query"):
                hints.add(row["query"])
            existing["categoryHints"] = sorted(h for h in hints if h)

    out = list(groups.values())
    output_path.write_text(json.dumps(out, ensure_ascii=False, indent=2), encoding="utf-8")
    print(f"input rows: {len(data)}")
    print(f"output rows: {len(out)}")
    print(f"saved: {output_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
