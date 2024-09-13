using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;  // Newtonsoft.Json�� ����ϱ� ���� ���ӽ����̽� �߰�

public class SpriteLoader : MonoBehaviour
{
    public Texture2D atlasTexture;  // �ؽ�ó ��Ʋ�� �̹���
    public TextAsset jsonData;      // JSON ������ �ҷ��� TextAsset
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

        // TextAsset�� �ؽ�Ʈ�� ���ڿ��� ��ȯ�Ͽ� JSON �����͸� �Ľ�
        if (jsonData == null || string.IsNullOrEmpty(jsonData.text))
        {
            Debug.LogError("JSON �����Ͱ� �����ϴ�. TextAsset�� Ȯ���ϼ���.");
            return;
        }

        string jsonString = jsonData.text;

        // JSON �����͸� AtlasData�� �Ľ� (Newtonsoft.Json ���)
        AtlasData atlasData = JsonConvert.DeserializeObject<AtlasData>(jsonString);
        sprites = new Dictionary<string, Sprite>();

        // frames �κ��� Dictionary�� �����Ǿ� �����Ƿ� �̸� Dictionary�� ó��
        foreach (var frameData in atlasData.frames)
        {
            int correctedY = atlasTexture.height - (frameData.Value.frame.y + frameData.Value.frame.h);
            Rect spriteRect = new Rect(frameData.Value.frame.x, correctedY, frameData.Value.frame.w, frameData.Value.frame.h);
            
            Sprite sprite = Sprite.Create(atlasTexture, spriteRect, new Vector2(0.5f, 0.5f));
            sprites.Add(frameData.Key, sprite);  // frameData.Key�� ��������Ʈ �̸�
        }

    }

    public Sprite GetSprite(string spriteName)
    {
        // ��������Ʈ �̸��� ����Ͽ� �ʿ��� ��������Ʈ ��ȯ
        if (sprites.ContainsKey(spriteName))
        {
            return sprites[spriteName];
        }
        return null;
    }
}

public class AtlasData
{
    public Dictionary<string, FrameData> frames;  // Dictionary�� ����
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
