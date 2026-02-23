#!/usr/bin/env python3
import argparse
import json
import re
import sys
from pathlib import Path


TYPE_RULES = [
    (("sushi", "ramen", "udon", "yakitori", "izakaya"), ("日式料理", "日式")),
    (("coffee", "cafe", "tea", "latte"), ("咖啡、簡餐、下午茶", "咖啡")),
    (("breakfast", "brunch"), ("早餐/早午餐", "早午餐")),
    (("bbq", "barbecue", "grill"), ("燒烤類", "燒烤")),
    (("fast food", "burger", "fried chicken"), ("速食料理", "速食")),
    (("dessert", "bakery", "cake", "ice cream"), ("烘焙、甜點、零食", "甜點")),
    (("hotpot", "hot pot"), ("鍋類", "火鍋")),
    (("vegan", "vegetarian"), ("素食", "素食")),
    (("thai", "vietnam", "malay", "indian"), ("亞洲料理(港、泰、印、星、馬)", "亞洲")),
    (("dumpling", "noodle", "rice", "taiwan", "chinese"), ("中式料理", "中式"))
]


def parse_args():
    p = argparse.ArgumentParser(description="Build Data/global_stores.json from deduped data")
    p.add_argument("--in", dest="input_path", required=True)
    p.add_argument("--out", dest="output_path", default="Data/global_stores.json")
    p.add_argument("--merge-existing", action="store_true")
    p.add_argument("--allow-empty-input", action="store_true")
    return p.parse_args()


def text_blob(item):
    parts = [
        item.get("name", ""),
        item.get("address", ""),
        " ".join(item.get("categoryHints", [])),
        item.get("query", "")
    ]
    return " ".join(str(x) for x in parts if x).lower()


def map_type(item):
    blob = text_blob(item)
    for keys, mapped in TYPE_RULES:
        if any(k in blob for k in keys):
            return mapped
    return ("其他美食", "其他")


def to_seed(item):
    c3, c4 = map_type(item)
    rating = item.get("rating")
    avg_price = item.get("averagePrice")
    return {
        "name": item.get("name", "").strip(),
        "phone": item.get("phone"),
        "lat": round(float(item.get("latitude")), 6),
        "lng": round(float(item.get("longitude")), 6),
        "rating": round(float(rating), 1) if rating is not None else None,
        "avgPrice": int(avg_price) if avg_price is not None else None,
        "c3": c3,
        "c4": c4
    }


def norm_name(name):
    s = (name or "").lower().strip()
    return re.sub(r"\s+", " ", s)


def seed_key(row):
    return f"{norm_name(row.get('name'))}|{round(float(row.get('lat', 0.0)),3)}|{round(float(row.get('lng', 0.0)),3)}"


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

    seeds = [to_seed(x) for x in data if x.get("name") and x.get("latitude") is not None and x.get("longitude") is not None]
    seeds = [x for x in seeds if x["name"]]

    if args.merge_existing and output_path.exists():
        existing = json.loads(output_path.read_text(encoding="utf-8"))
        merged = {}
        for row in existing + seeds:
            merged[seed_key(row)] = row
        seeds = list(merged.values())

    seeds.sort(key=lambda x: (x["name"].lower(), x["lat"], x["lng"]))
    output_path.write_text(json.dumps(seeds, ensure_ascii=False, indent=2), encoding="utf-8")

    print(f"input rows: {len(data)}")
    print(f"output rows: {len(seeds)}")
    print(f"saved: {output_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
