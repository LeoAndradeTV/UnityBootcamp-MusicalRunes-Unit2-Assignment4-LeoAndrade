using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assignment2 : MonoBehaviour
{
    // Initialize array of integers that can be changed in the inspector
    [SerializeField] int[] numbers = new[] {66, 77, 88, 99, 55, 44, 101};

    // Set time to wait between each log of the algorithm
    private float _timeToWait = 3f;

    // Start is called before the first frame update
    void Start()
    {
        // Run the algorithm right after play button is pressed
        FindLargestNumber(numbers);

        // Run the algorithm ever 3 seconds
        StartCoroutine(LargestNumberAfterSeconds(numbers, _timeToWait));
    }

    /// <summary>
    /// Finds the largest number in an array of integers.
    /// </summary>
    /// <param name="numbers">Array of integers.</param>
    private void FindLargestNumber(int[] numbers)
    {
        // Assume largest number is the first one in the array
        int largestNumber = numbers[0];

        // Set the index of the largest number to the firs position in the array
        int largestNumberIndex = 0;

        // Iterate through the array
        for (int i = 1; i < numbers.Length; i++)
        {
            // If number at current position is greater than previous largest number
            if (numbers[i] > largestNumber)
            {
                // Set the new largest number and the index which the number is at
                largestNumber = numbers[i];
                largestNumberIndex = i;
            }
        }
        // Print out information to the console
        Debug.Log($"The largest number is {largestNumber} at index {largestNumberIndex}");

        // Swap largest number with a random value between 0 and 999.
        numbers[largestNumberIndex] = Random.Range(0, 1000);
    }
    /// <summary>
    /// Runs the algorithm to find the largest number evey 3 seconds.
    /// </summary>
    /// <param name="numbers">Array to sort through.</param>
    /// <param name="timeToWait">Time to wait until next log.</param>
    /// <returns></returns>
    IEnumerator LargestNumberAfterSeconds(int[] numbers, float timeToWait)
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToWait);
            FindLargestNumber(numbers);
        }
    }
}
