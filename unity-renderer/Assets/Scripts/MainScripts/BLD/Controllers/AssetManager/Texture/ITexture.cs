using System;
using UnityEngine;

namespace BLD
{
    public interface ITexture : IDisposable
    {
        Texture2D texture { get; }
        int width { get; }
        int height { get; }
    }
}