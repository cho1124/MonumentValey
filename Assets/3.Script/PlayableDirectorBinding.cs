using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayableDirectorBinding : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public GameObject animationObject; // 애니메이션 트랙에 바인딩할 오브젝트
    public AudioSource audioSource;    // 오디오 트랙에 바인딩할 오브젝트
    public TimelineAsset timelineAsset;

    void Start()
    {
        // 타임라인 자산을 설정
        playableDirector.playableAsset = timelineAsset;

        // 타임라인 자산을 TimelineAsset으로 캐스팅하여 트랙을 검색
        TimelineAsset timeline = (TimelineAsset)playableDirector.playableAsset;

        // 타임라인의 트랙을 순회
        foreach (var track in timeline.GetOutputTracks())
        {
            // AnimationTrack에 오브젝트 바인딩
            if (track is AnimationTrack)
            {
                playableDirector.SetGenericBinding(track, animationObject);
                Debug.Log($"AnimationTrack에 {animationObject.name} 바인딩!");

            }
            // AudioTrack에 오브젝트 바인딩
            else if (track is AudioTrack)
            {
                playableDirector.SetGenericBinding(track, audioSource);
                Debug.Log($"AudioTrack에 {audioSource.name} 바인딩!");
            }

        }

        // PlayableDirector를 재생
        playableDirector.Play();
    }
}