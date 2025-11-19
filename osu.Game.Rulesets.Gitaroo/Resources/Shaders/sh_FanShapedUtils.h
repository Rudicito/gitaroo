// Inspired/copied from osu!framework sh_CircularProgressUtils.h

#ifndef FAN_SHAPED_UTILS_H
#define FAN_SHAPED_UTILS_H

#undef HALF_PI
#define HALF_PI 1.57079632679

highp float deltaToLine(
highp vec2 start,
highp vec2 end,
highp vec2 pixelPos)
{
    highp vec2 line = end - start;
    highp float len = length(line);
    highp vec2 n = vec2(-line.y, line.x);   // perpendicular vector

    // signed distance to the infinite line
    return dot(pixelPos - start, n) / len;
}

highp float deltaToLineGradient(highp float delta, highp float gradientLength){
    highp float halfGradientLength = gradientLength / 2;
    return 1.0 - smoothstep(-halfGradientLength, halfGradientLength, delta);
}

lowp vec3 getColour(
highp vec2 pixelPos,
highp vec3 trackedColour,
highp vec3 notTrackedColour,
highp float halfAngle,
highp float delta,
highp float gradientLength)
{
    // Optimization: If delta is 0, return full color immediately
    if (abs(delta) < 0.0001)
    return trackedColour;

    highp vec2 origin = vec2(0.5);

    highp float magnitude = abs(delta);

    highp float angle = halfAngle * magnitude;

    highp vec2 dir = vec2(
    cos(angle - HALF_PI),
    sin(angle - HALF_PI)
    );

    // NotTrackedColour coming from the left
    if (delta < 0)
    {
        dir.x = - dir.x;
        highp float g = deltaToLineGradient(deltaToLine(origin, origin + dir, pixelPos), gradientLength);
        return mix(notTrackedColour, trackedColour, g);
    }

    // NotTrackedColour coming from the right
    else
    {
        highp float g = deltaToLineGradient(deltaToLine(origin, origin + dir, pixelPos), gradientLength);
        return mix(trackedColour, notTrackedColour, g);
    }
}

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
    if (dist <= 0.5 - texelSize)
    {
        return mix(min_alpha, max_alpha, (0.5 - dist) / 0.5);
    }

    // Outside, too far
    else
    {
        lowp float edge = smoothstep(0.5, 0.5 - texelSize, dist);
        return edge * min_alpha;
    }
}

lowp float fanShapedAlphaAt(highp vec2 pixelPos, mediump float halfAngle, highp float texelSize, highp float linesWidth, mediump float linesAlpha, mediump float fanShapedMinAlpha, mediump float fanShapedMaxAlpha)
{
    highp vec2 origin = vec2(0.5);
    highp float radius = 0.5;

    mediump float pixelAngle = atan(0.5 - pixelPos.y, 0.5 - pixelPos.x) - HALF_PI;
    highp float halfLinesWidth = linesWidth * 0.5;

    highp float dist = distance(pixelPos, origin);

    highp vec2 csRight = vec2(cos(halfAngle - HALF_PI), sin(halfAngle - HALF_PI));
    highp vec2 csLeft = vec2(-csRight.x, csRight.y);

    highp vec2 edgeRight = origin + csRight * vec2(radius - texelSize - halfLinesWidth);
    highp float dstToEdgeRight = dstToLine(origin, edgeRight, pixelPos);

    highp vec2 edgeLeft = origin + csLeft * vec2(radius - texelSize - halfLinesWidth);
    highp float dstToEdgeLeft = dstToLine(origin, edgeLeft, pixelPos);

    highp float edgeDist = min(dstToEdgeLeft, dstToEdgeRight);

    if (abs(pixelAngle) < halfAngle)
    {
        // Inside sector
        lowp float centerAlpha = alphaAtFar(dist, texelSize, fanShapedMinAlpha, fanShapedMaxAlpha);
        lowp float edgeAlpha = alphaAtLines(edgeDist, texelSize, linesAlpha, halfLinesWidth);
        return max(centerAlpha, edgeAlpha); // Choose stronger effect
    }
    else
    {
        // Outside sector
        return alphaAtLines(edgeDist, texelSize, linesAlpha, halfLinesWidth);
    }
}

#endif
