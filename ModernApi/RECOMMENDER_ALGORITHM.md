# FoodieCare 推薦演算法升級說明 (V2)

## 1. 目標
- 保留原本規則樹的可解釋性（type 推薦）。
- 將店家排序從「距離優先」升級為「多因子可調權重排序」。
- 讓演算法可在 `appsettings.json` 調整，不需要改程式碼。

## 2. 架構總覽
推薦流程分成兩段：

1. `Type Candidate` 產生  
   - 使用 `RuleBasedTypeRecommender` 給出 `primaryType`。  
   - 融合關聯類型與使用者近期偏好，得到 `candidateTypes`。

2. `Store Ranking` 排序  
   - 先查資料庫（或 fallback seed）。  
   - 對候選店家逐筆計算 `Score`。  
   - 依 `Score DESC, Distance ASC` 排序回傳。

## 3. 排序公式
每間店分數：

`Score = w_d * DistanceScore + w_p * PriceScore + w_r * RatingScore + w_t * TypeScore + w_e * ExploreScore`

各分量：

- `DistanceScore`：距離越近越高，正規化到 0~1。  
- `PriceScore`：符合預算區間給高分，不符合給低分。  
- `RatingScore`：評分 / 5（無評分給中立分）。  
- `TypeScore`：有指定 type 時給高分；無指定時給中立分。  
- `ExploreScore`：用店名 hash 做穩定微隨機，避免推薦完全同質化。

## 4. 可調參數
位置：`FoodieCare:Ranking`

- `Enabled`：是否啟用加權排序。
- `DistanceWeight`：距離權重（建議 0.4~0.6）。
- `PriceWeight`：價位匹配權重（建議 0.15~0.3）。
- `RatingWeight`：評分權重（建議 0.1~0.25）。
- `TypeWeight`：類型權重（建議 0.05~0.2）。
- `ExplorationWeight`：探索權重（建議 0.02~0.1）。
- `ExplorationBucketCount`：探索分桶數（越大越細）。
- `DbFetchMultiplier`：DB 初次抓取倍數，用於排序前候選擴充（建議 3~6）。

## 5. 目前預設
```json
"Ranking": {
  "Enabled": true,
  "DistanceWeight": 0.5,
  "PriceWeight": 0.2,
  "RatingWeight": 0.2,
  "TypeWeight": 0.1,
  "ExplorationWeight": 0.05,
  "ExplorationBucketCount": 9,
  "DbFetchMultiplier": 4
}
```

## 5.1 A/B 參數組切換
目前內建三組：
- `balanced`：平衡版（預設）
- `conservative`：穩健版（更偏近距離與價位一致）
- `explore`：探索版（提高多樣性）

切換方式：
```json
"FoodieCare": {
  "ActiveRankingProfile": "explore"
}
```

重啟 API 後生效。  
可呼叫 `GET /api/seed/count` 查看 `activeRankingProfile`。

## 6. 回傳欄位
`StoreDto` 新增：
- `score`：後端排序分數，方便前端觀察與除錯。

## 7. 無資料與降級策略
- DB 連線失敗或查無資料：fallback 到 global seed。
- fallback 仍無結果：放寬條件（類型、距離）。
- 最後保底：回傳最近 seed 店家（避免 UI 空白）。

## 8. 建議驗證指標
上線後觀察：
- 推薦點擊率（CTR）
- 右滑率（Like Rate）
- 最終選店率（Choose Rate）
- 重複店家比例（多樣性）

## 9. 下一步升級建議
1. 加入時間特徵（營業時段、早餐/宵夜）。
2. 加入 user long-term embedding（長期口味向量）。
3. 做 A/B 測試不同權重配置，逐步收斂到最佳參數。
