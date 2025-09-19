// Inspired/copied from osu!framework sh_CircularProgressUtils.h

#ifndef FAN_SHAPED_UTILS_H
#define FAN_SHAPED_UTILS_H

#undef HALF_PI
#define HALF_PI 1.57079632679

highp float dstToLine(highp vec2 start, highp vec2 end, highp vec2 pixelPos)
{
    highp float lineLength = distance(end, start);

    if (lineLength < 0.001)
        return distance(pixelPos, start);

    highp vec2 a = (end - start) / lineLength;
    highp vec2 closest = clamp(dot(a, pixelPos - start), 0.0, distance(end, start)) * a + start; // closest point on a line from given position
    return distance(closest, pixelPos);
}

lowp float alphaAtLines(highp float dist, highp float texelSize, lowp float max_alpha, highp float halfLinesWidth)
{
    lowp float edge = 0.0;

    if (dist < halfLinesWidth - texelSize) {
        edge = 1.0;
    } else if (dist < halfLinesWidth + texelSize) {
        edge = smoothstep(halfLinesWidth + texelSize, halfLinesWidth - texelSize, dist);
    } else {
        edge = 0.0;
    }

    return mix(0.0, max_alpha, edge);
}

lowp float alphaAtFar(highp float dist, highp float texelSize, lowp float min_alpha, lowp float max_alpha)
{
    //  Inside the shape
    if (dist <= 0.5 - texelSize) {
        return mix(min_alpha, max_alpha, (0.5 - dist) / 0.5);
    }
            
    // Outside, too far
    else
    {
        lowp float edge = smoothstep(0.5, 0.5 - texelSize, dist);
        return edge * min_alpha;
    }
}

lowp float fanShapedAlphaAt(highp vec2 pixelPos, mediump float angle, highp float texelSize, highp float linesWidth, mediump float linesAlpha, mediump float fanShapedMinAlpha, mediump float fanShapedMaxAlpha)
{
    highp vec2 origin = vec2(0.5);
    highp float radius = 0.5;
    
    mediump float pixelAngle = atan(0.5 - pixelPos.y, 0.5 - pixelPos.x) - HALF_PI;
    mediump float halfAngle = radians(angle / 2);
    highp float halfLinesWidth = linesWidth * 0.5;

    highp float dist = distance(pixelPos, origin);

    highp vec2 csRight = vec2(cos(halfAngle - HALF_PI), sin(halfAngle - HALF_PI));
    highp vec2 csLeft = vec2(-csRight.x, csRight.y);
    
    highp vec2 edgeRight = origin + csRight * vec2(radius - texelSize - halfLinesWidth);
    highp float dstToEdgeRight = dstToLine(origin, edgeRight, pixelPos);

    highp vec2 edgeLeft = origin + csLeft * vec2(radius - texelSize - halfLinesWidth);
    highp float dstToEdgeLeft = dstToLine(origin, edgeLeft, pixelPos);

    highp float edgeDist = min(dstToEdgeLeft, dstToEdgeRight);

    if (abs(pixelAngle) < halfAngle) {
        // Inside sector
        lowp float centerAlpha = alphaAtFar(dist, texelSize, fanShapedMinAlpha, fanShapedMaxAlpha);
        lowp float edgeAlpha = alphaAtLines(edgeDist, texelSize, linesAlpha, halfLinesWidth);
        return max(centerAlpha, edgeAlpha); // Choose stronger effect
    } else {
        // Outside sector
        return alphaAtLines(edgeDist, texelSize, linesAlpha, halfLinesWidth);
    }
}

#endif
