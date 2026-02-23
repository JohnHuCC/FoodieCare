# Hybrid Places Architecture

## Goal
Provide a production-ready mixed data strategy for travel use cases:
- Open data baseline (`OSM / Overpass`)
- Commercial high-quality fallback (`Google Places`, optional)
- Unified merged API response for frontend

## Components
1. `OsmPlacesProvider`
- Source: Overpass API
- Strength: low cost, broad coverage
- Weakness: ratings/phone data can be sparse

2. `GooglePlacesProvider`
- Source: Google Places Nearby Search
- Strength: richer metadata, better POI quality
- Weakness: paid API

3. `HybridPlacesService`
- Calls enabled providers
- Merges and de-duplicates by normalized name + geo bucket
- Applies in-memory cache to reduce external API cost

## Endpoint
`POST /api/places/hybrid-search`

Request:
```json
{
  "query": "sushi",
  "latitude": 35.6762,
  "longitude": 139.6503,
  "radiusKm": 5,
  "limit": 40
}
```

Response fields:
- `name`, `address`, `phone`
- `latitude`, `longitude`
- `rating`, `averagePrice`
- `distanceKm`
- `source` (`osm` or `google`)

## Config
`FoodieCare:HybridPlaces`
- `Enabled`: master switch
- `UseOsm`: enable OSM provider
- `UseGoogle`: enable Google provider
- `GoogleApiKey`: Google key (required only when `UseGoogle=true`)
- `CacheMinutes`: cache TTL
- `MaxMergedResults`: output cap after merge
- `OsmFetchLimit`, `GoogleFetchLimit`: per-provider fetch caps

## Recommended rollout
1. Start with `UseOsm=true`, `UseGoogle=false`
2. Enable Google only for key markets/pages
3. Monitor cache hit rate and API cost
4. Persist merged results to DB for cold-start improvement
