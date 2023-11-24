using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheHangingHouse.Utility.Extensions;

namespace TheHangingHouse.UI.TableInternal
{
    public class TableRowsColoring : MonoBehaviour
    {
        [Header("Set In Inspector")]
        public Color[] rowsColors = { Color.gray * 1.5f, Color.gray};

        private Table _table;

        private void Awake()
        {
            _table = GetComponent<Table>();
            _table.onGenerate.AddListener(OnGenerate);
        }

        private void OnGenerate()
        {
            _table.cells.Foreach((cell, i, j) =>
                cell.Colour = rowsColors[i % rowsColors.Length]);
        }
    }
}

