using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DoodleBoard : MonoBehaviour, IDragHandler
{
    private RectTransform rectTransform;
    private Texture2D texture2D;

    private readonly int PAINTER_SIZE = 15;
    private readonly int WIDTH = 560;
    private readonly int HEIGHT = 560;
    private readonly int GRID_SIZE = 20;

    public bool showGrid = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        texture2D = new Texture2D(WIDTH, HEIGHT);
        Clear();

        var image = GetComponent<Image>();
        image.sprite = Sprite.Create(texture2D, new Rect(0, 0, WIDTH, HEIGHT), Vector2.zero);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, null, out Vector2 localPoint);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position - eventData.delta, null, out Vector2 lastPoint);

        for (int j = 0; j < 10; j++)
        {
            int textureX = (int)Mathf.Lerp(localPoint.x, lastPoint.x, j / 10f);
            int textureY = (int)Mathf.Lerp(localPoint.y, lastPoint.y, j / 10f);

            for (int x = textureX - PAINTER_SIZE; x < textureX + PAINTER_SIZE; x++)
            {
                for (int y = textureY - PAINTER_SIZE; y < textureY + PAINTER_SIZE; y++)
                {
                    var rect = rectTransform.rect;
                    if (rect.Contains(new Vector2(x, y)) == false)
                    {
                        continue;
                    }
                    texture2D.SetPixel(x, y, Color.red);
                }
            }
        }
        texture2D.Apply();
    }

    public void Clear()
    {
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (showGrid && (x % GRID_SIZE == 0 || y % GRID_SIZE == 0))
                {
                    texture2D.SetPixel(x, y, Color.blue);
                }
                else
                {
                    texture2D.SetPixel(x, y, Color.black);
                }
            }
        }

        texture2D.Apply();
    }

    public void PrintData()
    {
        // Be consistent with MNIST Data (28x28)
        var col = WIDTH / GRID_SIZE;    // 28
        var row = HEIGHT / GRID_SIZE;   // 28
        var space = (float)GRID_SIZE;
        var data = new float[col, row];

        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                float sum = 0;
                for (int i = (int)(x * space); i < (x * space) + space; i++)
                {
                    for (int j = (int)(y * space); j < (y * space) + space; j++)
                    {
                        sum += texture2D.GetPixel(i, j).r;
                    }
                }
                // average
                sum /= space * space;
                // revert y axis
                data[x, row - 1 - y] = sum;
            }
        }

        var str = "[";
        for (int i = 0; i < 28; i++)
        {
            str += "[";
            for (int j = 0; j < 28; j++)
            {
                if (j != 0)
                {
                    str += ", ";
                }
                str += data[j, i];
            }
            str += "],\n";
        }
        str += "]";
        Debug.Log(str);
    }
}
