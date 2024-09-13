using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;  // Newtonsoft.Json을 사용하기 위한 네임스페이스 추가

public class SpriteLoader : MonoBehaviour
{
    public Texture2D atlasTexture;  // 텍스처 아틀라스 이미지
    public TextAsset jsonData;      // JSON 파일을 불러올 TextAsset
    public static SpriteLoader instance = null;

    private Dictionary<string, Sprite> sprites;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // TextAsset의 텍스트를 문자열로 변환하여 JSON 데이터를 파싱
        if (jsonData == null || string.IsNullOrEmpty(jsonData.text))
        {
            Debug.LogError("JSON 데이터가 없습니다. TextAsset을 확인하세요.");
            return;
        }

        string jsonString = jsonData.text;

        // JSON 데이터를 AtlasData로 파싱 (Newtonsoft.Json 사용)
        AtlasData atlasData = JsonConvert.DeserializeObject<AtlasData>(jsonString);
        sprites = new Dictionary<string, Sprite>();

        // frames 부분이 Dictionary로 구성되어 있으므로 이를 Dictionary로 처리
        foreach (var frameData in atlasData.frames)
        {
            int correctedY = atlasTexture.height - (frameData.Value.frame.y + frameData.Value.frame.h);
            Rect spriteRect = new Rect(frameData.Value.frame.x, correctedY, frameData.Value.frame.w, frameData.Value.frame.h);
            
            Sprite sprite = Sprite.Create(atlasTexture, spriteRect, new Vector2(0.5f, 0.5f));
            sprites.Add(frameData.Key, sprite);  // frameData.Key는 스프라이트 이름
        }

    }

    public Sprite GetSprite(string spriteName)
    {
        // 스프라이트 이름을 사용하여 필요한 스프라이트 반환
        if (sprites.ContainsKey(spriteName))
        {
            return sprites[spriteName];
        }
        return null;
    }
}

public class AtlasData
{
    public Dictionary<string, FrameData> frames;  // Dictionary로 선언
    public MetaData meta;
}

public class FrameData
{
    public FrameRect frame;
    public bool rotated;
    public bool trimmed;
    public FrameSize spriteSourceSize;
    public FrameSize sourceSize;
}

public class FrameRect
{
    public int x;
    public int y;
    public int w;
    public int h;
}

public class FrameSize
{
    public int x;
    public int y;
    public int w;
    public int h;
}

public class MetaData
{
    public string app;
    public string version;
    public string image;
    public string format;
    public FrameSize size;
    public float scale;
    public string smartupdate;
}
