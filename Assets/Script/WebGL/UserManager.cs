using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }

    public List<RecentUserLocation> onlineUsers = new();
    public List<RecentUserLocation> offlineUsers = new();
    public Dictionary<int, List<RecentUserLocation>> onlineUsersByFloor = new();

    private DataFetcher fetcher;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        fetcher = FindObjectOfType<DataFetcher>();
    }

    void Update()
    {
        if (fetcher == null || fetcher.latestData == null || fetcher.latestData.Count == 0)
        {
            Debug.LogWarning("⚠️ DataFetcher가 null입니다.");
            return;
        }
            

        onlineUsers.Clear();
        offlineUsers.Clear();
        onlineUsersByFloor.Clear();

        Debug.Log($"📦 데이터 갯수: {fetcher.latestData.Count}");

        foreach (var user in fetcher.latestData)
        {
            Debug.Log($"▶️ 유저 {user.userId} 상태: {user.userStatus}, 층: {user.userFloor}");
            if (user.userStatus == "Active")
            {
                if (!onlineUsers.Any(u => u.userId == user.userId))
                {
                    onlineUsers.Add(user);
                    offlineUsers.RemoveAll(u => u.userId == user.userId);
                    Debug.Log($"🟢 온라인 추가: {user.userId}");
                    int rawFloor = user.userFloor;
                    int floor = (rawFloor == 0) ? 1 : rawFloor;
                    if (!onlineUsersByFloor.TryGetValue(floor, out var list))
                    {
                        list = new List<RecentUserLocation>();
                        onlineUsersByFloor[floor] = list;
                    }
                    list.Add(user);
                }
            }
            else
            {
                if (!offlineUsers.Any(u => u.userId == user.userId))
                {
                    offlineUsers.Add(user);
                    onlineUsers.RemoveAll(u => u.userId == user.userId);
                    Debug.Log($"🔴 오프라인 추가: {user.userId}");
                }
            }
            Debug.Log($"📊 최종 결과: 온라인 {onlineUsers.Count}명, 오프라인 {offlineUsers.Count}명");
        }
    }

    public int GetOnlineCount(int floor) =>
        onlineUsersByFloor.TryGetValue(floor, out var list) ? list.Count : 0;
}
