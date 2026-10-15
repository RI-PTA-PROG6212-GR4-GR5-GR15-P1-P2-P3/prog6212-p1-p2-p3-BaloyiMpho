using System;
using Testing.Models;
using Testing.Services;

public class Program
{
    public Calculations calc = new Calculations();
    public ArtClass artClass = new ArtClass();

    public static void Main()
    {
        Program solution = new Program();

        Console.WriteLine(solution.artClass.GetArt());

        Console.WriteLine("How many elements are in the first sorted array?");
         int size = int.Parse(Console.ReadLine());

        int[] nums2 = new int[size];
        int[] nums1 = new int[size];
        for (int i= 0; i<nums2.Length; i++)
        {
            nums1[i] = i;
        }
        Console.WriteLine(string.Join(", ", nums1));

    }

    
}