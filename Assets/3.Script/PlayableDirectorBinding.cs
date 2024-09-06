using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayableDirectorBinding : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public GameObject animationObject; // �ִϸ��̼� Ʈ���� ���ε��� ������Ʈ
    public AudioSource audioSource;    // ����� Ʈ���� ���ε��� ������Ʈ
    public TimelineAsset timelineAsset;

    void Start()
    {
        // Ÿ�Ӷ��� �ڻ��� ����
        playableDirector.playableAsset = timelineAsset;

        // Ÿ�Ӷ��� �ڻ��� TimelineAsset���� ĳ�����Ͽ� Ʈ���� �˻�
        TimelineAsset timeline = (TimelineAsset)playableDirector.playableAsset;

        // Ÿ�Ӷ����� Ʈ���� ��ȸ
        foreach (var track in timeline.GetOutputTracks())
        {
            // AnimationTrack�� ������Ʈ ���ε�
            if (track is AnimationTrack)
            {
                playableDirector.SetGenericBinding(track, animationObject);
                Debug.Log($"AnimationTrack�� {animationObject.name} ���ε�!");

            }
            // AudioTrack�� ������Ʈ ���ε�
            else if (track is AudioTrack)
            {
                playableDirector.SetGenericBinding(track, audioSource);
                Debug.Log($"AudioTrack�� {audioSource.name} ���ε�!");
            }

        }

        // PlayableDirector�� ���
        playableDirector.Play();
    }
}