using System.Collections;
using System.Collections.Generic;
using TheHangingHouse.UI.TableInternal;
using TheHangingHouse.Utility.Extensions;
using UnityEngine;

namespace TheHangingHouse.UI
{
    [RequireComponent(typeof(Table)), ExecuteAlways]
    public class TableCornersTextAligner : MonoBehaviour
    {
        private Table m_table;

        private void OnEnable()
        {
            m_table = GetComponent<Table>();
            m_table.onGenerate.AddListener(OnGenerateTable);
        }

        private void OnGenerateTable()
        {
            m_table.generator.parameters.titleRow.GetComponentsInChildren<Cell>().Foreach((cell, i) =>
            {
                if (i == 0)
                    cell.labelText.alignment = TMPro.TextAlignmentOptions.Left;
                if (i == m_table.generator.ColumnsCount - 1)
                    cell.labelText.alignment = TMPro.TextAlignmentOptions.Right;
            });
            m_table.generator.Cells.Foreach((cell, i, j) =>
            {
                if (j == 0)
                    cell.labelText.alignment = TMPro.TextAlignmentOptions.Left;
                if (j == m_table.generator.ColumnsCount - 1)
                    cell.labelText.alignment = TMPro.TextAlignmentOptions.Right;
            });
        }

    }
}

