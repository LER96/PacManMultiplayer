using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Sprite[] _sprites;
    [SerializeField] float _animationTime;
    int _animationFrame;
    bool _isLooping = true;

    private void Start()
    {
        InvokeRepeating(nameof(Animate), _animationTime, _animationTime);
    }

    public void Animate()
    {
        //if player dies, pause animation
        if (!this._spriteRenderer.enabled)
        {
            return;
        }

        //reset animation loop
        this._animationFrame++;
        if (this._animationFrame >= this._sprites.Length && this._isLooping)
        {
            this._animationFrame = 0;
        }

        //if we didnt finish all sprites, continue to next frame
        if (this._animationFrame >= 0 && this._animationFrame < this._sprites.Length)
        {
            this._spriteRenderer.sprite = this._sprites[this._animationFrame];
        }
    }

    public void RestartAnimation()
    {
        this._animationFrame = -1;
        Animate();
    }
}
