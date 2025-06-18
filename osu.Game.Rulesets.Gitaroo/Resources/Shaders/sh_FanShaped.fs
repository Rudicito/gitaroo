// Inspired/copied from osu!framework sh_CircularProgress.fs

#ifndef FAN_SHAPED_FS
#define FAN_SHAPED_FS

#undef HIGH_PRECISION_VERTEX
#define HIGH_PRECISION_VERTEX

#include "sh_Utils.h"
#include "sh_Masking.h"
#include "sh_TextureWrapping.h"
#include "sh_FanShapedUtils.h"

layout(location = 2) in highp vec2 v_TexCoord;

layout(std140, set = 0, binding = 0) uniform m_FanShapedParameters
{
    mediump float angle;
    highp float texelSize;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
    highp vec2 resolution = v_TexRect.zw - v_TexRect.xy;
    highp vec2 pixelPos = (v_TexCoord - v_TexRect.xy) / resolution;
    
    highp vec2 wrappedCoord = wrap(v_TexCoord, v_TexRect);
    lowp vec4 textureColour = getRoundedColor(wrappedSampler(wrappedCoord, v_TexRect, m_Texture, m_Sampler, -0.9), wrappedCoord);

    o_Colour = vec4(textureColour.rgb, textureColour.a * fanShapedAlphaAt(pixelPos, angle, texelSize));
}

#endif