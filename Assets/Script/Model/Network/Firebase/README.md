## Firebase Realtime DB 모듈 (간소화)

### 핵심: 3가지만 알면 됩니다

#### 1️⃣ 초기화 (OnAwake/Start에서 한 번)
```csharp
await FirebaseInitializer.EnsureInitializedAsync();
```

#### 2️⃣ 데이터 구독 (값이 변경되면 콜백)
```csharp
string id = await FirebaseClient.Instance.SubscribeAsync<MainGameRealtimeModel>(
    "rooms/room1/players/player1",
    onValueChanged: (model) => { /* 여기서 처리 */ }
);
```

#### 3️⃣ 구독 해제 (OnDestroy에서)
```csharp
FirebaseClient.Instance.Unsubscribe(id);
```

---

### 파일 구조
```
Assets/Script/Network/Firebase/
├── FirebaseInitializer.cs         ← 초기화 관리
├── FirebaseDatabaseClient.cs       ← 싱글턴 클라이언트
├── FirebaseClient.cs              ← 진짜 사용할 것
├── MainGameRealtimeModel.cs        ← 모델 (필요 시 수정)
└── FirebaseExample.cs             ← 복붙 예제
```

---

### 사용 예제 (그냥 복붙하면 됨)

```csharp
public class MyGameUI : MonoBehaviour
{
    private string _subId;

    private async void Start()
    {
        await FirebaseInitializer.EnsureInitializedAsync();
        
        _subId = await FirebaseClient.Instance.SubscribeAsync<MainGameRealtimeModel>(
            "rooms/room1/players/player1",
            onValueChanged: (data) =>
            {
                Debug.Log($"플레이어가 {data.selectMoveType}로 움직임");
                UpdateUI(data);
            }
        );
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(_subId))
            FirebaseClient.Instance.Unsubscribe(_subId);
    }

    private void UpdateUI(MainGameRealtimeModel data)
    {
        // UI 업데이트 로직
    }
}
```

---

### 주의사항
- ❌ `Repository`, `Factory` 같은 복잡한 것들은 무시해도 됨
- ✅ `FirebaseClient.Instance`만 사용하면 됨
- ✅ 모델 필드 추가는 `MainGameRealtimeModel.cs`에서만
- ✅ Firebase 경로는 자유롭게 설정
