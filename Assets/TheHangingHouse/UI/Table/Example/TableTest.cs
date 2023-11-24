using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheHangingHouse.Utility.Extensions;

namespace TheHangingHouse.UI.TableInternal
{
    public class TableTest : MonoBehaviour
    {
        [Header("Set In Inspector")]
        public Table table;

        [Header("Graphic Paramters")]
        public Color[] rowsColors;

        private void Start()
        {
            var people = new Person[]
            {
                new Person { Name = "Mohammad", Age = 19, Sex = "Male", Length = 176 },
                new Person { Name = "Ahmad", Age = 18, Sex = "Male", Length = 174 },
                new Person { Name = "Omar", Age = 22, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
                new Person { Name = "Yamen", Age = 21, Sex = "Male", Length = 173 },
            };

            table.Generate(people);
            table.generator.Cells.Foreach((cell, i, j) => cell.Colour = rowsColors[i % rowsColors.Length]); 
        }
    }

    public class Person
    {
        [TableProperty] public string Name { get; set; }
        [TableProperty] public int Age { get; set; }
        [TableProperty] public string Sex { get; set; }
        [TableProperty] public float Length { get; set; }
    } 

}
