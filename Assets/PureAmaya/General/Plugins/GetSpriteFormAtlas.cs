using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

/// <summary>
/// 从EasyAtlasToSprite中获取图片
/// </summary>
public class GetSpriteFormAtlas : MonoBehaviour
{
    public SpriteAtlas spriteAtlas;
    public SpriteRenderer spriteRenderer;
    public Image image;
    public string SpriteName;
    public bool DestroyAfterGettingSprite = true;

    // Start is called before the first frame update
    [ContextMenu("GetSprite")]
    private void Start()
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.sprite= spriteAtlas.GetSprite(SpriteName);
        }
        else if(image != null)
        {
            image.sprite = spriteAtlas.GetSprite(SpriteName);
        }

       if(DestroyAfterGettingSprite && Application.isPlaying) Destroy(this);
    }
}
