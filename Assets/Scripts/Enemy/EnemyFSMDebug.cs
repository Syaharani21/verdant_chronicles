// ============================================================
//  EnemyFSMDebug.cs
//  Tampilkan state FSM di atas musuh saat runtime (Editor only).
//  Pasang di GameObject yang sama dengan EnemyFSM / BossFSM.
// ============================================================

using UnityEngine;

public class EnemyFSMDebug : MonoBehaviour
{
#if UNITY_EDITOR
    private EnemyFSM   enemyFSM;
    private BossFSM    bossFSM;
    private GUIStyle   labelStyle;

    private static readonly Color[] stateColors = new Color[]
    {
        new Color(0.4f, 0.8f, 0.4f), // Patrol  — hijau
        new Color(0.9f, 0.8f, 0.2f), // Chase   — kuning
        new Color(0.9f, 0.3f, 0.3f), // Attack  — merah
        new Color(0.4f, 0.6f, 1.0f), // Retreat — biru
        new Color(0.4f, 0.4f, 0.4f), // Dead    — abu
    };

    private void Awake()
    {
        enemyFSM = GetComponent<EnemyFSM>();
        bossFSM  = GetComponent<BossFSM>();
    }

    private void OnGUI()
    {
        if (Camera.main == null) return;

        EnemyState state;
        string     extra = "";

        if (enemyFSM != null)
        {
            state = enemyFSM.CurrentState;
            extra = $"\nAggr: {enemyFSM.AdaptData.aggressionLevel}  Hits: {enemyFSM.AdaptData.playerHitCount}";
        }
        else if (bossFSM != null)
        {
            state = bossFSM.CurrentState;
            extra = $"\nFase {bossFSM.CurrentPhase}";
        }
        else return;

        // Konversi world → screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.2f);
        if (screenPos.z < 0) return; // Di belakang kamera

        // Unity GUI (0,0) ada di kiri atas, tapi WorldToScreenPoint (0,0) di kiri bawah
        screenPos.y = Screen.height - screenPos.y;

        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
        }

        string label   = $"[{state}{extra}]";
        Color  bgColor = stateColors[(int)state];

        // Background kotak
        Rect rect = new Rect(screenPos.x - 50, screenPos.y - 12, 100, extra.Length > 0 ? 36 : 18);
        GUI.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0.75f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);

        // Label teks
        GUI.color = Color.black;
        GUI.Label(rect, label, labelStyle);
        GUI.color = Color.white;
    }
#endif
}
