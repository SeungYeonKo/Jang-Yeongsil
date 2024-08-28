using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Tile
{
  public enum Direction
  {
    UP, DOWN, LEFT, RIGHT,
  }
  public enum PosNegType
  {
    POS,
    NEG,
    NONE,
  }

  // 타일 패딩 값 (타일 경계와 실제 이미지 사이의 거리)
  public static int padding = 20;

  // 타일의 크기(픽셀)
  public static int tileSize = 100;

  // 각 방향과 곡선 유형에 대한 LineRenderer를 저장하는 딕셔너리
  private Dictionary<(Direction, PosNegType), LineRenderer> mLineRenderers
    = new Dictionary<(Direction, PosNegType), LineRenderer>();

  // 템플릿 베지어 곡선 제어점으로부터 생성된 곡선 점들을 저장
  public static List<Vector2> BezCurve =
    BezierCurve.PointList2(TemplateBezierCurve.templateControlPoints, 0.001f);

  // 직소 타일을 생성하기 위한 원본 텍스처
  private Texture2D mOriginalTexture;

  public Texture2D finalCut { get; private set; }

  public static readonly Color TransparentColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

  private PosNegType[] mCurveTypes = new PosNegType[4]
  {
    PosNegType.NONE,
    PosNegType.NONE,
    PosNegType.NONE,
    PosNegType.NONE,
  };

  // 플러드 필을 위한 방문 여부를 저장하는 2차원 배열
  // 다차원 배열의 어떤 칸과 연결된 영역을 찾는 알고리즘
    private bool[,] mVisited;

  // 플러드 필을 위한 스택
  private Stack<Vector2Int> mStack = new Stack<Vector2Int>();

  public int xIndex = 0;
  public int yIndex = 0;

  // 타일 정렬을 위한 클래스
  public static TilesSorting tilesSorting = new TilesSorting();
  public void SetCurveType(Direction dir, PosNegType type)
  {
    mCurveTypes[(int)dir] = type;
  }

  public PosNegType GetCurveType(Direction dir)
  {
    return mCurveTypes[(int)dir];
  }

  public Tile(Texture2D texture)
  {
    mOriginalTexture = texture;
    int tileSizeWithPadding = 2 * padding + tileSize;

    // 투명한 색상으로 초기화된 타일 텍스처를 생성
    finalCut = new Texture2D(tileSizeWithPadding, tileSizeWithPadding, TextureFormat.ARGB32, false);
    for (int i = 0; i < tileSizeWithPadding; ++i)
    {
      for (int j = 0; j < tileSizeWithPadding; ++j)
      {
        finalCut.SetPixel(i, j, TransparentColor);
      }
    }
  }

  public void Apply()
  {
    FloodFillInit();
    FloodFill();
    finalCut.Apply();
  }

  void FloodFillInit()
  {
    int tileSizeWithPadding = 2 * padding + tileSize;

    mVisited = new bool[tileSizeWithPadding, tileSizeWithPadding];
    for (int i = 0; i < tileSizeWithPadding; ++i)
    {
      for (int j = 0; j < tileSizeWithPadding; ++j)
      {
        mVisited[i, j] = false;
      }
    }

    List<Vector2> pts = new List<Vector2>();
    for (int i = 0; i < mCurveTypes.Length; ++i)
    {
      pts.AddRange(CreateCurve((Direction)i, mCurveTypes[i]));
    }

    // 닫힌 곡선이 생성되면 해당 점들을 방문으로 표시
    for (int i = 0; i < pts.Count; ++i)
    {
      mVisited[(int)pts[i].x, (int)pts[i].y] = true;
    }
    // 중앙에서 시작하여 플러드 필을 수행
    Vector2Int start = new Vector2Int(tileSizeWithPadding / 2, tileSizeWithPadding / 2);
    mVisited[start.x, start.y] = true;
    mStack.Push(start);
  }

  void Fill(int x, int y)
  {
    Color c = mOriginalTexture.GetPixel(x + xIndex * tileSize, y + yIndex * tileSize);
    c.a = 1.0f;
    finalCut.SetPixel(x, y, c);
  }

  void FloodFill()
  {
    int width_height = padding * 2 + tileSize;

    while (mStack.Count > 0)
    {
      Vector2Int v = mStack.Pop();

      int xx = v.x;
      int yy = v.y;

      Fill(v.x, v.y);

      // 오른쪽 확인
      int x = xx + 1;
      int y = yy;

      if (x < width_height)
      {
        Color c = finalCut.GetPixel(x, y);
        if (!mVisited[x, y])
        {
          mVisited[x, y] = true;
          mStack.Push(new Vector2Int(x, y));
        }
      }

      // 왼쪽 확인
      x = xx - 1;
      y = yy;
      if (x > 0)
      {
        Color c = finalCut.GetPixel(x, y);
        if (!mVisited[x, y])
        {
          mVisited[x, y] = true;
          mStack.Push(new Vector2Int(x, y));
        }
      }

      // 위
      x = xx;
      y = yy + 1;

      if (y < width_height)
      {
        Color c = finalCut.GetPixel(x, y);
        if (!mVisited[x, y])
        {
          mVisited[x, y] = true;
          mStack.Push(new Vector2Int(x, y));
        }
      }

      // 아래
      x = xx;
      y = yy - 1;

      if (y >= 0)
      {
        Color c = finalCut.GetPixel(x, y);
        if (!mVisited[x, y])
        {
          mVisited[x, y] = true;
          mStack.Push(new Vector2Int(x, y));
        }
      }
    }
  }

  public static LineRenderer CreateLineRenderer(UnityEngine.Color color, float lineWidth = 1.0f)
  {
    GameObject obj = new GameObject();
    LineRenderer lr = obj.AddComponent<LineRenderer>();

    lr.startColor = color;
    lr.endColor = color;
    lr.startWidth = lineWidth;
    lr.endWidth = lineWidth;
    lr.material = new Material(Shader.Find("Sprites/Default"));
    return lr;
  }

  public static void TranslatePoints(List<Vector2> iList, Vector2 offset)
  {
    for (int i = 0; i < iList.Count; i++)
    {
      iList[i] += offset;
    }
  }

  public static void InvertY(List<Vector2> iList)
  {
    for (int i = 0; i < iList.Count; i++)
    {
      iList[i] = new Vector2(iList[i].x, -iList[i].y);
    }
  }

  public static void SwapXY(List<Vector2> iList)
  {
    for (int i = 0; i < iList.Count; ++i)
    {
      iList[i] = new Vector2(iList[i].y, iList[i].x);
    }
  }

  public List<Vector2> CreateCurve(Direction dir, PosNegType type)
  {
    int padding_x = padding;
    int padding_y = padding;
    int sw = tileSize;
    int sh = tileSize;

    List<Vector2> pts = new List<Vector2>(BezCurve);
    switch (dir)
    {
      case Direction.UP:
        if (type == PosNegType.POS)
        {
          TranslatePoints(pts, new Vector2(padding_x, padding_y + sh));
        }
        else if (type == PosNegType.NEG)
        {
          InvertY(pts);
          TranslatePoints(pts, new Vector2(padding_x, padding_y + sh));
        }
        else
        {
          pts.Clear();
          for (int i = 0; i < 100; ++i)
          {
            pts.Add(new Vector2(i + padding_x, padding_y + sh));
          }
        }
        break;
      case Direction.RIGHT:
        if (type == PosNegType.POS)
        {
          SwapXY(pts);
          TranslatePoints(pts, new Vector2(padding_x + sw, padding_y));
        }
        else if (type == PosNegType.NEG)
        {
          InvertY(pts);
          SwapXY(pts);
          TranslatePoints(pts, new Vector2(padding_x + sw, padding_y));
        }
        else
        {
          pts.Clear();
          for (int i = 0; i < 100; ++i)
          {
            pts.Add(new Vector2(padding_x + sw, i + padding_y));
          }
        }
        break;
      case Direction.DOWN:
        if (type == PosNegType.POS)
        {
          InvertY(pts);
          TranslatePoints(pts, new Vector2(padding_x, padding_y));
        }
        else if (type == PosNegType.NEG)
        {
          TranslatePoints(pts, new Vector2(padding_x, padding_y));
        }
        else
        {
          pts.Clear();
          for (int i = 0; i < 100; ++i)
          {
            pts.Add(new Vector2(i + padding_x, padding_y));
          }
        }
        break;
      case Direction.LEFT:
        if (type == PosNegType.POS)
        {
          InvertY(pts);
          SwapXY(pts);
          TranslatePoints(pts, new Vector2(padding_x, padding_y));
        }
        else if (type == PosNegType.NEG)
        {
          SwapXY(pts);
          TranslatePoints(pts, new Vector2(padding_x, padding_y));
        }
        else
        {
          pts.Clear();
          for (int i = 0; i < 100; ++i)
          {
            pts.Add(new Vector2(padding_x, i + padding_y));
          }
        }
        break;
    }
    return pts;
  }

  public void DrawCurve(Direction dir, PosNegType type, UnityEngine.Color color)
  {
    if (!mLineRenderers.ContainsKey((dir, type)))
    {
      mLineRenderers.Add((dir, type), CreateLineRenderer(color));
    }

    LineRenderer lr = mLineRenderers[(dir, type)];
    lr.gameObject.SetActive(true);
    lr.startColor = color;
    lr.endColor = color;
    lr.gameObject.name = "LineRenderer_" + dir.ToString() + "_" + type.ToString();
    List<Vector2> pts = CreateCurve(dir, type);

    lr.positionCount = pts.Count;
    for (int i = 0; i < pts.Count; ++i)
    {
      lr.SetPosition(i, pts[i]);
    }
  }

  public void HideAllCurves()
  {
    foreach (var item in mLineRenderers)
    {
      item.Value.gameObject.SetActive(false);
    }
  }

  public void DestroyAllCurves()
  {
    foreach (var item in mLineRenderers)
    {
      GameObject.Destroy(item.Value.gameObject);
    }

    mLineRenderers.Clear();
  }

}
