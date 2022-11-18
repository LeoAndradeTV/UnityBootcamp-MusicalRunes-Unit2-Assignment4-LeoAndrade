using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBehaviour2 : MonoBehaviour
{
    [SerializeField] private Transform[] _locationPoints;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private bool _reverseOrder;

    private int _targetIndex = 0;
    private Vector3 _target;
    private Vector3 _currentLocation;
    private bool _isAtBeginning = true;

    /// <summary>
    /// Update position by moving towards a specified target
    /// </summary>
    void Update()
    {
        _currentLocation = transform.position;
        _target = _locationPoints[_targetIndex].position;
        transform.position = Vector3.MoveTowards(_currentLocation, _target, _speed * Time.deltaTime);

        if (_currentLocation == _target)
        {
            ChangeTarget();
        }
    }

    /// <summary>
    /// Change the target without hard coded indexes so user only needs to add new targets in the Inspector.
    /// </summary>
    void ChangeTarget()
    {
        // Check to see if order is in reverse
        if (!_reverseOrder)
        {
            // While we are not at the end of the List, keep adding to the index
            if (_targetIndex < _locationPoints.Length - 1)
            {
                _targetIndex++;
            }
            // When we get to the end of the List, set index back to the beginning
            else if (_targetIndex == _locationPoints.Length - 1)
            {
                _targetIndex = 0;
            }
        } else
        {
            // Check to see the direction we are going
            if (_isAtBeginning)
            {
                if (_targetIndex < _locationPoints.Length - 1)
                {
                    _targetIndex++;
                }
                // If at the end of the list, switch directions
                else if (_targetIndex == _locationPoints.Length - 1)
                {
                    _isAtBeginning = false;
                    _targetIndex--;
                }
            }
            // If we are going in reverse order
            else
            {
                // While we are not in the beginning, keep decreasing the index
                if (_targetIndex > 0)
                {
                    _targetIndex--;
                }
                // Once we get to the beginning, change directions
                else if (_targetIndex == 0)
                {
                    _isAtBeginning = true;
                }
            }

        }
    }

    //With Switch-Case but hard-coded indexes.

    //void ChangeTarget()
    //{
    //    if (!_reverseOrder)
    //    {
    //        switch (_targetIndex)
    //        {
    //            case 0: _targetIndex = 1; break;
    //            case 1: _targetIndex = 2; break;
    //            case 2: _targetIndex = 3; break;
    //            case 3: _targetIndex = 0; break;
    //        }
    //    } else
    //    {
    //        switch (_targetIndex)
    //        {
    //            case 0:
    //                _isAtBeginning = true;
    //                _targetIndex = 1; 
    //                break;
    //            case 1:
    //                if (_isAtBeginning)
    //                {
    //                    _targetIndex = 2;
    //                }
    //                else
    //                {
    //                    _targetIndex = 0;
    //                }
    //                break;
    //            case 2:
    //                if (_isAtBeginning)
    //                {
    //                    _targetIndex = 3;
    //                }
    //                else
    //                {
    //                    _targetIndex = 1;
    //                }
    //                break;
    //            case 3:
    //                _isAtBeginning = false;
    //                _targetIndex = 2;
    //                break;
    //        }
    //    }
    //}
}
