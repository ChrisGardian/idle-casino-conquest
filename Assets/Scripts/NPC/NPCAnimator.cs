using System.Collections;
using UnityEngine;

public class NPCAnimator : MonoBehaviour
{
    [Header("NPC Sprites")]
    public Sprite[] npcIdleFrames;
    public Sprite[] npcWalkFrames;
    public Sprite[] npcPlayFrames;
    public Sprite[] npcWinFrames;

    [Header("VIP Sprites")]
    public Sprite[] vipIdleFrames;
    public Sprite[] vipWalkFrames;
    public Sprite[] vipPlayFrames;
    public Sprite[] vipWinFrames;

    [Header("Settings")]
    public float frameDuration = 0.15f;

    private NPC _npc;
    private SpriteRenderer _sr;
    private Sprite[] _currentAnim;
    private int _frame;
    private float _timer;
    private bool _playingWin;

    private Sprite[] IdleFrames => _npc.data.isVIP ? vipIdleFrames : npcIdleFrames;
    private Sprite[] WalkFrames => _npc.data.isVIP ? vipWalkFrames : npcWalkFrames;
    private Sprite[] PlayFrames => _npc.data.isVIP ? vipPlayFrames : npcPlayFrames;
    private Sprite[] WinFrames  => _npc.data.isVIP ? vipWinFrames  : npcWinFrames;

    void Awake()
    {
        _npc = GetComponentInParent<NPC>();
        _sr = GetComponent<SpriteRenderer>();
    }

    public void TriggerWin() => StartCoroutine(PlayWinOnce());

    void Update()
    {
        if (_playingWin) return;

        Sprite[] target = _npc.State switch
        {
            NPCState.Walking => WalkFrames,
            NPCState.Leaving => _npc.IsAtMachine ? PlayFrames : WalkFrames,
            NPCState.Playing => PlayFrames,
            _                => IdleFrames,
        };

        _sr.flipX = _npc.MovingRight;

        if (_currentAnim != target)
        {
            _currentAnim = target;
            _frame = 0;
            _timer = 0f;
        }

        if (_currentAnim == null || _currentAnim.Length == 0) return;

        _timer += Time.deltaTime;
        if (_timer >= frameDuration)
        {
            _timer = 0f;
            _frame = (_frame + 1) % _currentAnim.Length;
            _sr.sprite = _currentAnim[_frame];
        }
    }

    private IEnumerator PlayWinOnce()
    {
        if (WinFrames == null || WinFrames.Length == 0) yield break;

        _playingWin = true;
        foreach (var frame in WinFrames)
        {
            _sr.sprite = frame;
            yield return new WaitForSeconds(frameDuration);
        }
        _playingWin = false;
    }
}
