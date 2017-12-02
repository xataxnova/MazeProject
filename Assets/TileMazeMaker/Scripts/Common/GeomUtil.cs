using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 2016-12-6
/// Created by HawkWang.
/// Free to use for any purpose.
/// Thanks to http://dev.gameres.com/Program/Abstract/Geometry.htm#矢量的概念
/// 目录
/// 点是否在直线上：PointOnLine
/// 点是否在线段上：PointOnSegment
/// 两个线段是否相交：SegmentsCross
/// 点是否在多边形之内：PointInsidePolygon
/// 点是否在圆内：PointInCircle
/// 求垂直法线：Perpendicular
/// 点到线段的最近点:NearestPointToSegment
/// 
/// </summary>
public class GeomUtil
{
    public const float Epsilon = 0.0000001f;

    //按照10e-7精度计算 0值。
    public static bool NearlyZero(float val) 
    {
        return Mathf.Abs(val) < Epsilon;
    }

    public static bool PointOnLine(Vector2 start, Vector2 end, Vector2 test_point) 
    {
        return NearlyZero( (test_point.x - start.x) * (end.y - start.y) - (test_point.y - start.y) * (end.x - start.x) );
    }

    /// <summary>
    /// 测试一个点，是不是在一个线段上
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="test_point"></param>
    /// <returns></returns>
    public static bool PointOnSegment(Vector2 start, Vector2 end, Vector2 test_point)
    {
        if ( PointOnLine( start, end, test_point ) == true )
        {
            if (test_point.x >= Mathf.Min(start.x, end.x) &&
                test_point.x <= Mathf.Max(start.x, end.x) &&
                test_point.y >= Mathf.Min(start.y, end.y) &&
                test_point.y <= Mathf.Max(start.y, end.y)
                )
            {
                return true;
            }
        }

        return false;
    }

    ///判断两个线段是否相交
    public static bool SegmentsCross(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {

        float denominator = ((b.x - a.x) * (d.y - c.y)) - ((b.y - a.y) * (d.x - c.x));

        if (denominator == 0)
            return false;

        float numerator1 = ((a.y - c.y) * (d.x - c.x)) - ((a.x - c.x) * (d.y - c.y));
        float numerator2 = ((a.y - c.y) * (b.x - a.x)) - ((a.x - c.x) * (b.y - a.y));

        if (numerator1 == 0 || numerator2 == 0)
            return false;

        float r = numerator1 / denominator;
        float s = numerator2 / denominator;

        return (r > 0 && r < 1) && (s > 0 && s < 1);
    }

    ///判断一个点是不是在一个多边形内，可以支持非凸多边形
    ///已知点P，和参考点O，点O在多边形之外，如果OP与多边行的每个边有奇数个交点，那么P在多边形内部，否则，P在多边形外部
    public static bool PointInsidePolygon(Vector2[] polyPoints, Vector2 point)
    {

        float xMin = 0;
        for (int i = 0; i < polyPoints.Length; i++)
            xMin = Mathf.Min(xMin, polyPoints[i].x);

        Vector2 origin = new Vector2(xMin - 0.1f, point.y);
        int intersections = 0;

        for (int i = 0; i < polyPoints.Length; i++)
        {

            Vector2 pA = polyPoints[i];
            Vector2 pB = polyPoints[(i + 1) % polyPoints.Length];

            if (GeomUtil.SegmentsCross(origin, point, pA, pB))
                intersections++;
        }

        return (intersections & 1) == 1;
    }

    //点是否在圆内
    public static bool PointInCircle(Vector2 center, float radius, Vector2 test_point) 
    {
        return (test_point - center).sqrMagnitude < radius * radius;
    }

    //求平面内，与已知法向量垂直的两个法向量。
    //x1 = x0*cos90 + y0 * sin90
    //y1 = -x0*sin90 + y0* cos90
    public static Vector2[] Perpendicular(Vector2 normalized) 
    {
        Vector2 [] result = new Vector2[2];
        result[0] = new Vector2(-normalized.y, normalized.x);
        result[1] = new Vector2(normalized.y, -normalized.x);

        return result;       
    }

    //圆1是否在圆2之内
    public static bool Circle1InCircle2(Vector2 c1, float r1, Vector2 c2, float r2) 
    {
        if (r1 > r2) return false;

        return (c1 - c2).sqrMagnitude < (r2 - r1) * (r2 - r1);
    }

    //点到线段的最近点
    public static Vector2 NearestPointToSegment(Vector2 seg_P1, Vector2 seg_P2, Vector2 test_point) 
    {
        Vector2 perpendicular_foot = Vector2.zero;

        if (seg_P1.y == seg_P2.y) 
        {
            perpendicular_foot = new Vector2(test_point.x, seg_P1.y);
        }
        else if (seg_P1.x == seg_P2.x)
        {
            perpendicular_foot = new Vector2(seg_P1.x, test_point.y);
        }
        else 
        {
            float k = (seg_P2.y - seg_P1.y) / (seg_P2.x - seg_P1.x);

            //x = ( k^2 * pt1.x + k * (point.y - pt1.y ) + point.x ) / ( k^2 + 1) ，y = k * ( x - pt1.x) + pt1.y

            perpendicular_foot.x = (k * k * seg_P1.x + k * (test_point.y - seg_P1.y) + test_point.x) / (k * k + 1);
            perpendicular_foot.y = k * (perpendicular_foot.x - seg_P1.x) + seg_P1.y;
        }

        if (PointOnSegment(seg_P1, seg_P2, perpendicular_foot))
        {
            return perpendicular_foot;
        }
        else 
        {
            return (seg_P1 - test_point).sqrMagnitude < (seg_P2 - test_point).sqrMagnitude ? seg_P1 : seg_P2;
        }
    }

    //判断多边形的某个点，是不是凹点
    public static bool PointIsConcave(Vector2[] points, int point)
    {

        Vector2 current = points[point];
        Vector2 next = points[(point + 1) % points.Length];
        Vector2 previous = points[point == 0 ? points.Length - 1 : point - 1];

        Vector2 left = new Vector2(current.x - previous.x, current.y - previous.y);
        Vector2 right = new Vector2(next.x - current.x, next.y - current.y);

        float cross = (left.x * right.y) - (left.y * right.x);

        return cross > 0;
    }
}

