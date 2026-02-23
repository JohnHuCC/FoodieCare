# FoodieCare Migration Plan (WebForms -> ASP.NET Core)

## Current decision

Use parallel migration: keep `WebSite2` online, move traffic endpoint-by-endpoint to `ModernApi`.

## Phase 1 (Completed)

- Add `ModernApi` .NET 8 service skeleton
- Implement rule-based recommendation engine from `food_tree_rule.txt`
- Add MySQL repository with parameterized SQL
- Expose API endpoints for recommendation options and store search

## Phase 2 (Completed in this pass)

- Add auth endpoints: register/login with bearer token
- Add interaction endpoints:
  - click tracking (`user_click`)
  - recommendation feedback insert (`food_question_form_test`)
  - user preference update (`user_data`) by selected store
- Keep compatibility with current DB schema and category naming

## Phase 3 (In progress)

- Added minimal web frontend under `ModernApi/wwwroot`:
  - `index.html` (register/login)
  - `main.html` (location + entry)
  - `recommender.html` (recommendation + store list + click tracking)
  - `browse.html` (nearby food browsing + click tracking)
- Next:
  - Replace basic pages with production UI + form validation
  - Decommission Web Forms pages (`recommender.aspx`, `recommend_result.aspx`, `food_browser.aspx`)

## Critical risks to fix before production

- Legacy and new system still use plaintext password (must migrate to hashed password)
- Legacy SQL injection paths still exist in `WebSite2`
- Legacy static mutable fields can cause cross-user leakage
- Existing DB credentials in old source should be rotated
