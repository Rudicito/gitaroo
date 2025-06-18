// Inspired/copied from osu!framework sh_CircularProgressUtils.h

#ifndef FAN_SHAPED_UTILS_H
#define FAN_SHAPED_UTILS_H

#undef PI
#define PI 3.1415926536

#undef HALF_PI
#define HALF_PI 1.57079632679

#undef TWO_PI
#define TWO_PI 6.28318530718

highp float dstToLine(highp vec2 start, highp vec2 end, highp vec2 pixelPos)
{
    highp float lineLength = distance(end, start);

    if (lineLength < 0.001)
        return distance(pixelPos, start);

    highp vec2 a = (end - start) / lineLength;
    highp vec2 closest = clamp(dot(a, pixelPos - start), 0.0, distance(end, start)) * a + start; // closest point on a line from given position
    return distance(closest, pixelPos);
}

lowp float alphaAt(highp float dist, highp float texelSize, lowp float min_alpha, lowp float max_alpha)
{
    lowp float subAAMultiplier = clamp(1.0 / (texelSize * 2.0), 0.1, 1.0);

    lowp float edge = smoothstep(texelSize, 0.0, dist) * subAAMultiplier;

    return min_alpha + edge * (max_alpha - min_alpha);
}

lowp float alphaAtFar(highp float dist, highp float texelSize, lowp float min_alpha, lowp float max_alpha)
{

    //  For inside the shape, 0.5 is the radius of the circle, vec(0.5f) is the center of the circle
    if (dist <= 0.5) {
        // Interpolate from min_alpha at edge to max_alpha at center
        return min_alpha + (0.5 - dist) * (max_alpha - min_alpha);
    }
    // For the edges
    else
    {
        // Smooth transition at the edge - from min_alpha to 0
        lowp float edge = smoothstep(0.5 + texelSize, 0.5, dist);
        return edge * min_alpha;
    }
    
    return 0.f;
}

lowp float fanShapedAlphaAt(highp vec2 pixelPos, mediump float angle, highp float texelSize)
{
    mediump float pixelAngle = atan(0.5 - pixelPos.y, 0.5 - pixelPos.x) - HALF_PI;

    mediump float halfAngle = radians(angle / 2);
    highp float halfTexel = texelSize * 0.5;

    if (abs(pixelAngle) < halfAngle) // Pixel inside the sector
    {
        highp float dist = distance(pixelPos, vec2(0.5));
        return alphaAtFar(dist, texelSize, 0.1, 1.0);
    }

    // Calculate edge vectors rotated to align with vertical axis
    highp vec2 csRight = vec2(cos(halfAngle - HALF_PI), sin(halfAngle - HALF_PI));
    highp vec2 csLeft = vec2(cos(-halfAngle - HALF_PI), sin(-halfAngle - HALF_PI));

    // Now the edges will properly extend from center along the fan boundaries
    highp vec2 edgeRight = vec2(0.5) + vec2(0.5) * csRight;
    highp float dstToEdgeRight = dstToLine(vec2(0.5), edgeRight, pixelPos);

    highp vec2 edgeLeft = vec2(0.5) + vec2(0.5) * csLeft;
    highp float dstToEdgeLeft = dstToLine(vec2(0.5), edgeLeft, pixelPos);

    return alphaAt(min(dstToEdgeLeft, dstToEdgeRight), texelSize, 0.0, 1.0);
}

#endif
