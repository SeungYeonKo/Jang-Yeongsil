using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BoardGen : MonoBehaviour
{
  private string imageFilename;
  Sprite mBaseSpriteOpaque;
  Sprite mBaseSpriteTransparent;

  GameObject mGameObjectOpaque;
  GameObject mGameObjectTransparent;

  public float ghostTransparency = 0.1f;

  // 직소 타일 생성 관련 변수
  public int numTileX { get; private set; }
  public int numTileY { get; private set; }

  Tile[,] mTiles = null;
  GameObject[,] mTileGameObjects= null;

  public Transform parentForTiles = null;

  public Menu menu = null;
  private List<Rect> regions = new List<Rect>();
  private List<Coroutine> activeCoroutines = new List<Coroutine>();

  Sprite LoadBaseTexture()
  {
    Texture2D tex = SpriteUtils.LoadTexture(imageFilename);
    if (!tex.isReadable)
    {
      Debug.Log("Error: Texture is not readable");
      return null;
    }

    if (tex.width % Tile.tileSize != 0 || tex.height % Tile.tileSize != 0)
    {
      Debug.Log("Error: Image must be of size that is multiple of <" + Tile.tileSize + ">");
      return null;
    }

    // 이미지에 패딩을 추가
    Texture2D newTex = new Texture2D(
        tex.width + Tile.padding * 2,
        tex.height + Tile.padding * 2,
        TextureFormat.ARGB32,
        false);

    // 기본 색상을 흰색으로 설정
    for (int x = 0; x < newTex.width; ++x)
    {
      for (int y = 0; y < newTex.height; ++y)
      {
        newTex.SetPixel(x, y, Color.white);
      }
    }

    // 색상을 복사
    for (int x = 0; x < tex.width; ++x)
    {
      for (int y = 0; y < tex.height; ++y)
      {
        Color color = tex.GetPixel(x, y);
        color.a = 1.0f;
        newTex.SetPixel(x + Tile.padding, y + Tile.padding, color);
      }
    }
    newTex.Apply();

    Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
        newTex,
        0,
        0,
        newTex.width,
        newTex.height);
    return sprite;
  }

  void Start()
  {
    imageFilename = GameApp.Instance.GetJigsawImageName();

    mBaseSpriteOpaque = LoadBaseTexture();
    mGameObjectOpaque = new GameObject();
    mGameObjectOpaque.name = imageFilename + "_Opaque";
    mGameObjectOpaque.AddComponent<SpriteRenderer>().sprite = mBaseSpriteOpaque;
    mGameObjectOpaque.GetComponent<SpriteRenderer>().sortingLayerName = "Opaque";

    mBaseSpriteTransparent = CreateTransparentView(mBaseSpriteOpaque.texture);
    mGameObjectTransparent = new GameObject();
    mGameObjectTransparent.name = imageFilename + "_Transparent";
    mGameObjectTransparent.AddComponent<SpriteRenderer>().sprite = mBaseSpriteTransparent;
    mGameObjectTransparent.GetComponent<SpriteRenderer>().sortingLayerName = "Transparent";

    mGameObjectOpaque.gameObject.SetActive(false);

    SetCameraPosition();

    //CreateJigsawTiles();
    StartCoroutine(Coroutine_CreateJigsawTiles());
  }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CompletePuzzle();
        }
        if (GameApp.Instance.TotalTilesInCorrectPosition == mTileGameObjects.Length)
        {
            OnPuzzleCompleted(); 
        }
    }
  Sprite CreateTransparentView(Texture2D tex)
  {
    Texture2D newTex = new Texture2D(
      tex.width,
      tex.height, 
      TextureFormat.ARGB32, 
      false);

    for(int x = 0; x < newTex.width; x++)
    {
      for(int y = 0; y < newTex.height; y++)
      {
        Color c = tex.GetPixel(x, y);
        if(x > Tile.padding && 
           x < (newTex.width - Tile.padding) &&
           y > Tile.padding && 
           y < (newTex.height - Tile.padding))
        {
          c.a = ghostTransparency;
        }
        newTex.SetPixel(x, y, c);
      }
    }
    newTex.Apply();

    Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
      newTex,
      0,
      0,
      newTex.width,
      newTex.height);
    return sprite;
  }

  void SetCameraPosition()
  {
    Camera.main.transform.position = new Vector3(mBaseSpriteOpaque.texture.width / 2,
      mBaseSpriteOpaque.texture.height / 2, -10.0f);
    //Camera.main.orthographicSize = mBaseSpriteOpaque.texture.width / 2;
    int smaller_value = Mathf.Min(mBaseSpriteOpaque.texture.width, mBaseSpriteOpaque.texture.height);
    Camera.main.orthographicSize = smaller_value * 0.8f;
  }

  public static GameObject CreateGameObjectFromTile(Tile tile)
  {
    GameObject obj = new GameObject();

    obj.name = "TileGameObe_" + tile.xIndex.ToString() + "_" + tile.yIndex.ToString();

    obj.transform.position = new Vector3(tile.xIndex * Tile.tileSize, tile.yIndex * Tile.tileSize, 0.0f);

    SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
    spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(
      tile.finalCut,
      0,
      0,
      Tile.padding * 2 + Tile.tileSize,
      Tile.padding * 2 + Tile.tileSize);

    BoxCollider2D box = obj.AddComponent<BoxCollider2D>();

    TileMovement tileMovement = obj.AddComponent<TileMovement>();
    tileMovement.tile = tile;

    return obj;
  }

  void CreateJigsawTiles()
  {
    Texture2D baseTexture = mBaseSpriteOpaque.texture;
    numTileX = baseTexture.width / Tile.tileSize;
    numTileY = baseTexture.height / Tile.tileSize;

    mTiles = new Tile[numTileX, numTileY];
    mTileGameObjects = new GameObject[numTileX, numTileY];

    for(int i = 0; i < numTileX; i++)
    {
      for(int j = 0; j < numTileY; j++)
      {
        mTiles[i, j] = CreateTile(i, j, baseTexture);
        mTileGameObjects[i, j] = CreateGameObjectFromTile(mTiles[i, j]);
        if(parentForTiles != null)
        {
          mTileGameObjects[i, j].transform.SetParent(parentForTiles);
        }
      }
    }

    menu.SetEnableBottomPanel(true);
    menu.SetEnablePanelGameMode(true);
    menu.btnPlayOnClick = ShuffleTiles;
  }

  IEnumerator Coroutine_CreateJigsawTiles()
  {
    Texture2D baseTexture = mBaseSpriteOpaque.texture;
    numTileX = baseTexture.width / Tile.tileSize;
    numTileY = baseTexture.height / Tile.tileSize;

    mTiles = new Tile[numTileX, numTileY];
    mTileGameObjects = new GameObject[numTileX, numTileY];

    for (int i = 0; i < numTileX; i++)
    {
      for (int j = 0; j < numTileY; j++)
      {
        mTiles[i, j] = CreateTile(i, j, baseTexture);
        mTileGameObjects[i, j] = CreateGameObjectFromTile(mTiles[i, j]);
        if (parentForTiles != null)
        {
          mTileGameObjects[i, j].transform.SetParent(parentForTiles);
        }

        yield return null;
      }
    }

    // 아래 패널을 활성화. 플레이 버튼 클릭 시 델리게이트 설정
    menu.SetEnableBottomPanel(true);
    //menu.SetEnablePanelGameMode(true);
    menu.btnPlayOnClick = ShuffleTiles;

  }


  Tile CreateTile(int i, int j, Texture2D baseTexture)
  {
    Tile tile = new Tile(baseTexture);
    tile.xIndex = i;
    tile.yIndex = j;

    // 왼쪽 가장자리 타일의 경우
    if (i == 0)
    {
      tile.SetCurveType(Tile.Direction.LEFT, Tile.PosNegType.NONE);
    }
    else
    {
      // 왼쪽 타일의 반대 곡선 타입을 설정
      Tile leftTile = mTiles[i - 1, j];
      Tile.PosNegType rightOp = leftTile.GetCurveType(Tile.Direction.RIGHT);
      tile.SetCurveType(Tile.Direction.LEFT, rightOp == Tile.PosNegType.NEG ?
        Tile.PosNegType.POS : Tile.PosNegType.NEG);
    }

    // 아래쪽 가장자리 타일의 경우
    if (j == 0)
    {
      tile.SetCurveType(Tile.Direction.DOWN, Tile.PosNegType.NONE);
    }
    else
    {
      Tile downTile = mTiles[i, j - 1];
      Tile.PosNegType upOp = downTile.GetCurveType(Tile.Direction.UP);
      tile.SetCurveType(Tile.Direction.DOWN, upOp == Tile.PosNegType.NEG ?
        Tile.PosNegType.POS : Tile.PosNegType.NEG);
    }

    // 오른쪽 가장자리 타일의 경우
    if (i == numTileX - 1)
    {
      tile.SetCurveType(Tile.Direction.RIGHT, Tile.PosNegType.NONE);
    }
    else
    {
      float toss = UnityEngine.Random.Range(0f, 1f);
      if(toss < 0.5f)
      {
        tile.SetCurveType(Tile.Direction.RIGHT, Tile.PosNegType.POS);
      }
      else
      {
        tile.SetCurveType(Tile.Direction.RIGHT, Tile.PosNegType.NEG);
      }
    }

    // 위쪽 가장자리 타일
    if (j == numTileY - 1)
    {
      tile.SetCurveType(Tile.Direction.UP, Tile.PosNegType.NONE);
    }
    else
    {
      float toss = UnityEngine.Random.Range(0f, 1f);
      if (toss < 0.5f)
      {
        tile.SetCurveType(Tile.Direction.UP, Tile.PosNegType.POS);
      }
      else
      {
        tile.SetCurveType(Tile.Direction.UP, Tile.PosNegType.NEG);
      }
    }

    tile.Apply();
    return tile;
  }



  #region Shuffling related codes

  private IEnumerator Coroutine_MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
  {
    float elaspedTime = 0.0f;
    Vector3 startingPosition = objectToMove.transform.position;
    while(elaspedTime < seconds)
    {
      objectToMove.transform.position = Vector3.Lerp(
        startingPosition, end, (elaspedTime / seconds));
      elaspedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
    }
    objectToMove.transform.position = end;
  }

  void Shuffle(GameObject obj)
  {
    if(regions.Count == 0)
    {
      regions.Add(new Rect(-300.0f, -100.0f + 100.0f, 100.0f, numTileY * Tile.tileSize - 200.0f));
      regions.Add(new Rect((numTileX+1) * Tile.tileSize, -100.0f + 100.0f, 100.0f, numTileY * Tile.tileSize - 200.0f));
    }

    int regionIndex = UnityEngine.Random.Range(0, regions.Count);
    float x = UnityEngine.Random.Range(regions[regionIndex].xMin, regions[regionIndex].xMax);
    float y = UnityEngine.Random.Range(regions[regionIndex].yMin, regions[regionIndex].yMax);

    Vector3 pos = new Vector3(x, y, 0.0f);
    Coroutine moveCoroutine = StartCoroutine(Coroutine_MoveOverSeconds(obj, pos, 1.0f));
    activeCoroutines.Add(moveCoroutine);
  }

  IEnumerator Coroutine_Shuffle()
  {
    for(int i = 0; i < numTileX; ++i)
    {
      for(int j = 0; j < numTileY; ++j)
      {
        Shuffle(mTileGameObjects[i, j]);
        yield return null;
      }
    }

    foreach(var item in activeCoroutines)
    {
      if(item != null)
      {
        yield return null;
      }
    }

    OnFinishedShuffling();
  }

  public void ShuffleTiles()
  {
    StartCoroutine(Coroutine_Shuffle());
  }

  void OnFinishedShuffling()
  {
    activeCoroutines.Clear();

    menu.SetEnableBottomPanel(false);
    menu.SetEnablePanelGameMode(false);
    StartCoroutine(Coroutine_CallAfterDelay(() => menu.SetEnableTopPanel(true), 1.0f));
    GameApp.Instance.TileMovementEnabled = true;

    StartTimer();

    for(int i = 0; i < numTileX; ++i)
    {
      for(int j = 0; j < numTileY; ++j)
      {
        TileMovement tm = mTileGameObjects[i, j].GetComponent<TileMovement>();
        tm.onTileInPlace += OnTileInPlace;
        SpriteRenderer spriteRenderer = tm.gameObject.GetComponent<SpriteRenderer>();
        Tile.tilesSorting.BringToTop(spriteRenderer);
      }
    }

    menu.SetTotalTiles(numTileX * numTileY);
  }

  IEnumerator Coroutine_CallAfterDelay(System.Action function, float delay)
  {
    yield return new WaitForSeconds(delay);
    function();
  }


  public void StartTimer()
  {
    StartCoroutine(Coroutine_Timer());
  }

  IEnumerator Coroutine_Timer()
  {
    while(true)
    {
      yield return new WaitForSeconds(1.0f);
      GameApp.Instance.SecondsSinceStart += 1;

      menu.SetTimeInSeconds(GameApp.Instance.SecondsSinceStart);
    }
  }

  public void StopTimer()
  {
    StopCoroutine(Coroutine_Timer());
  }

  #endregion

  public void ShowOpaqueImage()
  {
    mGameObjectOpaque.SetActive(true);
  }

  public void HideOpaqueImage()
  {
    mGameObjectOpaque.SetActive(false);
  }

  void OnTileInPlace(TileMovement tm)
  {
    SoundManager.instance.PlaySfx(SoundManager.Sfx.PuzzleInPlace);
    GameApp.Instance.TotalTilesInCorrectPosition += 1;

    tm.enabled = false;
    Destroy(tm);

    SpriteRenderer spriteRenderer = tm.gameObject.GetComponent<SpriteRenderer>();
    Tile.tilesSorting.Remove(spriteRenderer);

    if (GameApp.Instance.TotalTilesInCorrectPosition == mTileGameObjects.Length)
    {
            OnPuzzleCompleted();
        }
    menu.SetTilesInPlace(GameApp.Instance.TotalTilesInCorrectPosition);
  }

    public void CompletePuzzle(bool callOnPuzzleCompleted = true)
    {
        // 모든 타일을 올바른 위치로 이동
        for (int i = 0; i < numTileX; i++)
        {
            for (int j = 0; j < numTileY; j++)
            {
                mTileGameObjects[i, j].transform.position = new Vector3(i * Tile.tileSize, j * Tile.tileSize, 0.0f);

                // 타일을 올바른 위치로 인식하도록 처리
                TileMovement tm = mTileGameObjects[i, j].GetComponent<TileMovement>();
                if (tm != null)
                {
                    tm.enabled = false;
                    Destroy(tm);
                }

                SpriteRenderer spriteRenderer = mTileGameObjects[i, j].GetComponent<SpriteRenderer>();
                Tile.tilesSorting.Remove(spriteRenderer);
            }
        }

        // 타일의 총 개수를 올바른 위치에 놓인 타일로 설정
        GameApp.Instance.TotalTilesInCorrectPosition = numTileX * numTileY;

        // 퍼즐이 완료된 것처럼 처리
        if (callOnPuzzleCompleted)
        {
            OnPuzzleCompleted();
        }
    }

    void OnPuzzleCompleted()
    {
        Hashtable customProperties = new Hashtable
        {
            { "StarMiniGameOver", true }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        menu.SetEnableTopPanel(false);
        menu.SetEnableGameCompletionPanel(true);

        // 값 초기화
        GameApp.Instance.SecondsSinceStart = 0;
        GameApp.Instance.TotalTilesInCorrectPosition = 0;
    }
}
