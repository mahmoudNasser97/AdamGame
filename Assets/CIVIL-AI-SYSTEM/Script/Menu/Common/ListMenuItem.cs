#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AISystem.Menu
{
    class ListMenuItem
    {
        float height, width, columnWidth, rowHeight;
        readonly int columns;
        readonly string[] columnHeader;
        Vector2 regionScrollPosition;
        Rect currentRow;
        float lastX = 0;
        List<float> values;

        Color[] colors = { Color.red, Color.white, Color.green };

        public ListMenuItem(Vector2 allocatedSpace, float rowHeight, string[] columnHeader, float[] values)
        {
            this.width = allocatedSpace.x;
            this.height = allocatedSpace.y;
            this.columns = columnHeader.Length;
            this.regionScrollPosition = new Vector2(0, 0);

            this.values = values == null ? new List<float>() : new List<float>(values);

            this.columnWidth = (width-15) / columns;
            this.rowHeight = rowHeight;
            this.columnHeader = columnHeader;
            
        }

        public Rect GetRect()
        {
            if (!EnoughSpaceInCurrentRow())
            {
                try
                {
                    currentRow = GUILayoutUtility.GetRect(columnWidth, rowHeight);
                    lastX = 0;
                }
                catch (System.Exception e)
                {
                    if (e.Message.Length > 5) { };
                }
            }

            return GetNextRectInCurrentRow();
        }

        bool EnoughSpaceInCurrentRow()
        {
            return currentRow.width >= lastX + columnWidth;
        }

        Rect GetNextRectInCurrentRow()
        {
            try
            {
                var ret = new Rect(currentRow) { x = lastX, width = columnWidth };
                lastX += columnWidth;
                return ret;
            }
            catch(System.Exception e)
            {
                if (e.Message.Length > 5) { };
                return new Rect(0, 0, 0, 0);
            }
        }

        public float[] GetValues()
        {
            return values.ToArray();
        }

        public void UpdateRendering(Vector2 allocatedSpace)
        {
            this.width = allocatedSpace.x;
            this.height = allocatedSpace.y;
            this.columnWidth = (width - 15) / columns;
        }

        public void Render(float[] updateValues = null)
        {                                                   GUILayout.Height(Mathf.Min(10 * values.Count, 340));
            for (int row = -1; row < values.Count + 1; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (row < values.Count)
                    {
                        if (row == -1)
                        {
                            EditorGUI.LabelField(GetRect(), columnHeader[column]);
                        }
                        else
                        {
                            switch (column)
                            {
                                case 0:
                                    EditorGUI.LabelField(GetRect(), "Id" + (row + 1) + ":");
                                    break;
                                case 1:
                                    values[row] = EditorGUI.FloatField(GetRect(), values[row]);
                                    break;
                                case 2:
                                    if(EditorGUI.LinkButton(GetRect(), "X"))
                                    {
                                        values.Remove(values[row]);
                                    };
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (column)
                        {
                            case 2:
                                if(EditorGUI.LinkButton(GetRect(), "Add"))
                                {
                                    values.Add(0);
                                }
                                break;
                            default:
                                GetRect();
                                break;
                        }
                    }
                }
            }
        }
    }
}

#endif