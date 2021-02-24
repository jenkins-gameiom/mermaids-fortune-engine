using System;
using System.Collections.Generic;
using System.Text;
using AGS.Slots.MermaidsFortune.Common.Interfaces;

namespace AGS.Slots.MermaidsFortune.Common.Helpers
{
    public class BonusGameHelper
    {
        public static List<RandomNumber> GenerateRandomNumbers(int specialValueIndex)
        {
            return new List<RandomNumber>(new RandomNumber[]
            {
                new RandomNumber() {Min = 0, Max = 57, Quantity = 1, Values = new List<int>(new int[] {56})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {specialValueIndex})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {57})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {57})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                new RandomNumber() {Min = 0, Max = 71, Quantity = 1, Values = new List<int>(new int[] {65})},
                new RandomNumber() {Min = 0, Max = 111, Quantity = 1, Values = new List<int>(new int[] {60})},
                new RandomNumber() {Min = 0, Max = 104, Quantity = 1, Values = new List<int>(new int[] {63})}
            });
        }
    }
}
