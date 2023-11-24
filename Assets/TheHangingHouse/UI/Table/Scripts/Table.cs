using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using TheHangingHouse.Utility.Extensions;
using TheHangingHouse.UI.TableInternal;

namespace TheHangingHouse.UI
{
    public class Table : MonoBehaviour
    {
        [Header("Set In Inspector")]
        public Generator.Parameters generatorParameters;

        [Header("Events")]
        public UnityEvent onGenerate;

        public Cell[,] cells;

        [SerializeField] public Generator generator;

        public void Clear()
        {
            var childCount = generatorParameters.rowsContainer.childCount;
            foreach (var child in generatorParameters.rowsContainer.Childs())
                DestroyImmediate(child.gameObject);
            if (generatorParameters.titleRow != null)
            {
                var cells = generatorParameters.titleRow.GetComponentsInChildren<Cell>();
                foreach (var cell in cells)
                    DestroyImmediate(cell.gameObject);
            }
        }

        public void Generate<T>(IEnumerable<T> elements)
            where T : class
        {
            generator?.Clear();
            generator = new Generator<T>(generatorParameters);
            generator.Generate(elements);
            cells = generator.Cells;
            onGenerate?.Invoke();
        }

        public void Generate(Dictionary<string, string>[] elements)
        {
            generator?.Clear();
            generator = new DictionaryGenerator(generatorParameters);
            generator.Generate(elements);
            cells = generator.Cells;
            onGenerate?.Invoke();
        }

        public abstract class Generator
        {
            public Parameters parameters;

            public abstract Cell[,] Cells { get; }
            public abstract IEnumerable<object> Elements { get; protected set; }
            public abstract List<Transform> Rows { get; protected set; }

            public int RowsCount => Rows.Count;
            public abstract int ColumnsCount { get; }

            protected Generator(Parameters parameters)
            {
                this.parameters = parameters;
            }


            public abstract void Generate(IEnumerable<object> elements);

            public abstract void Clear();

            [System.Serializable]
            public class Parameters
            {
                public RectTransform titleRow;
                public RectTransform rowsContainer;
                public GameObject prefabTableRow;
                public GameObject prefabTableTitleCell;
                public GameObject prefabTableCell;
            }
        }

        public class Generator<T> : Generator
            where T : class
        {
            public readonly PropertyInfo[] properties;

            public override Cell[,] Cells => _cells;

            public override IEnumerable<object> Elements { get; protected set; }
            public override List<Transform> Rows { get; protected set; } = new List<Transform>();
            public override int ColumnsCount => _columnsCount;

            private Cell[,] _cells;
            private int _columnsCount;

            public Generator(Parameters paramters) : base(paramters)
            {
                properties = typeof(T).GetProperties()
                    .Filter(prop => prop.GetCustomAttribute<TableProperty>() != null);
                _columnsCount = properties.Length;
            }

            public override void Generate(IEnumerable<object> elements)
            {
                Generate(elements.Select(obj => (T)obj));
                Elements = elements;
            }

            public void Generate(IEnumerable<T> elements)
            {
                _cells = new Cell[elements.Count(), properties.Length];

                if (parameters.titleRow != null)
                    FillRow(parameters.titleRow, null);

                int index = 0;
                foreach(var element in elements)
                {
                    var rowGO = Instantiate(parameters.prefabTableRow, parameters.rowsContainer);
                    var rowCells = FillRow(rowGO.transform, element);
                    for (int i = 0; i < rowCells.Length; i++)
                        _cells[index, i] = rowCells[i];
                    index++;
                    Rows.Add(rowGO.transform);
                }
            }

            private Cell[] FillRow(Transform row, T element)
            {
                var values = element != null ? properties.Map(prop => prop.GetValue(element)) : null;
                var names = properties.Map(prop => prop.Name);
                var cells = new Cell[properties.Length];

                var prefabCell = element == null ? parameters.prefabTableTitleCell : parameters.prefabTableCell;

                for (int i = 0; i < properties.Length; i++)
                {
                    var cellGO = Instantiate(prefabCell, row.GetChild(0));
                    var cell = cellGO.GetComponent<Cell>();
                    cell.Content = element != null ? $"{values[i]}" : $"{names[i].AddSpaces()}";
                    cells[i] = cell;
                }

                return cells;
            }

            public override void Clear()
            {
                var childCount = parameters.rowsContainer.childCount;
                var childs = parameters.rowsContainer.Childs();
                for (int i = 0; i < childCount; i++)
                    DestroyImmediate(childs[i].gameObject);
                    
                if (parameters.titleRow != null)
                {
                    var cells = parameters.titleRow.GetComponentsInChildren<Cell>();
                    foreach (var cell in cells)
                        DestroyImmediate(cell.gameObject);
                }
                _cells = null;
            }
        }

        public class DictionaryGenerator : Generator
        {
            public override Cell[,] Cells => _cells;
            public override IEnumerable<object> Elements { get; protected set; }
            public override List<Transform> Rows { get; protected set; }
            public override int ColumnsCount => _columnsCount;

            private Cell[,] _cells;
            private int _columnsCount;

            public DictionaryGenerator(Parameters parameters) : base(parameters) 
            {
                Rows = new List<Transform>();
            }

            public override void Generate(IEnumerable<object> elements)
            {
                Generate(elements.Select(obj => (Dictionary<string, string>)obj).ToArray());
                Elements = elements;
            }

            public void Generate(Dictionary<string, string>[] elements)
            {
                if (elements.Length == 0) return;

                _cells = new Cell[elements.Count(), elements[0].Keys.Count];

                if (parameters.titleRow != null)
                    FillRow(parameters.titleRow, elements[0], true);

                int index = 0;
                foreach (var element in elements)
                {
                    var rowGO = Instantiate(parameters.prefabTableRow, parameters.rowsContainer);
                    var rowCells = FillRow(rowGO.transform, element);
                    _columnsCount = rowCells.Length;
                    for (int i = 0; i < rowCells.Length; i++)
                        _cells[index, i] = rowCells[i];
                    index++;
                    Rows.Add(rowGO.transform);
                }
            }

            private Cell[] FillRow(Transform row, Dictionary<string, string> element, bool titleRow = false)
            {
                var values = element.Values.ToArray();
                var names = element.Keys.ToArray();
                var cells = new Cell[names.Length];

                var prefabCell = titleRow ? parameters.prefabTableTitleCell : parameters.prefabTableCell;

                for (int i = 0; i < cells.Length; i++)
                {
                    Debug.Log($"{row.gameObject.name}: {row.childCount}");
                    var cellGO = Instantiate(prefabCell, row.GetChild(0));
                    var cell = cellGO.GetComponent<Cell>();
                    cell.Content = !titleRow ? $"{values[i]}" : $"{names[i]}";
                    cells[i] = cell;
                }

                return cells;
            }

            public override void Clear()
            {
                var childCount = parameters.rowsContainer.childCount;
                for (int i = 0; i < childCount; i++)
                    DestroyImmediate(parameters.rowsContainer.GetChild(i).gameObject);
                if (parameters.titleRow != null)
                {
                    var cells = parameters.titleRow.GetComponentsInChildren<Cell>();
                    foreach (var cell in cells)
                        DestroyImmediate(cell.gameObject);
                }
                _cells = null;
            }
        }
    }
}