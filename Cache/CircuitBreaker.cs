// <copyright file="CircuitBreaker.cs" project="Cache">
// 
//    Copyright (c) MicroFrame Solutions. All rights reserved.
//    Author:    berkkulaksiz
//    CreatedAt:   18.05.2025
//    UpdatedAt: 18.05.2025
// 
//    Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// 
// </copyright>

namespace Cache;

/// <summary>
///     Circuit breaker implementation to prevent repeated failures.
/// </summary>
public class CircuitBreaker
{
    private readonly TimeSpan _durationOfBreak;
    private readonly int _exceptionsAllowedBeforeBreaking;

    private readonly object _lockObject = new();
    private DateTimeOffset? _circuitOpenedTime;
    private int _currentExceptionCount;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CircuitBreaker" /> class.
    /// </summary>
    /// <param name="exceptionsAllowedBeforeBreaking">Number of exceptions allowed before the circuit breaks.</param>
    /// <param name="durationOfBreak">Duration of the circuit break.</param>
    public CircuitBreaker(int exceptionsAllowedBeforeBreaking, TimeSpan durationOfBreak)
    {
        _exceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
        _durationOfBreak = durationOfBreak;
    }

    /// <summary>
    ///     Gets a value indicating whether the circuit is open.
    /// </summary>
    public bool IsOpen
    {
        get
        {
            lock (_lockObject)
            {
                if (_circuitOpenedTime.HasValue)
                {
                    // Check if the circuit has been open long enough to try again
                    if (DateTimeOffset.UtcNow - _circuitOpenedTime.Value > _durationOfBreak)
                    {
                        // Half-open state, allow one request through
                        _circuitOpenedTime = null;
                        _currentExceptionCount = 0;
                        return false;
                    }

                    return true;
                }

                return false;
            }
        }
    }

    /// <summary>
    ///     Gets a value indicating whether the circuit is closed.
    /// </summary>
    public bool IsClosed => !IsOpen;

    /// <summary>
    ///     Tracks an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public void TrackException(Exception exception)
    {
        // Avoid tracking specific non-critical exceptions
        if (exception is ArgumentException || exception is InvalidOperationException) return;

        lock (_lockObject)
        {
            _currentExceptionCount++;

            if (_currentExceptionCount >= _exceptionsAllowedBeforeBreaking)
                // Open the circuit
                _circuitOpenedTime = DateTimeOffset.UtcNow;
        }
    }

    /// <summary>
    ///     Resets the circuit breaker.
    /// </summary>
    public void Reset()
    {
        lock (_lockObject)
        {
            _currentExceptionCount = 0;
            _circuitOpenedTime = null;
        }
    }

    /// <summary>
    ///     Manually opens the circuit.
    /// </summary>
    public void Open()
    {
        lock (_lockObject)
        {
            _circuitOpenedTime = DateTimeOffset.UtcNow;
            _currentExceptionCount = _exceptionsAllowedBeforeBreaking;
        }
    }

    /// <summary>
    ///     Executes an action with circuit breaker protection.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <returns>The result of the action.</returns>
    /// <exception cref="BrokenCircuitException">Thrown when the circuit is open.</exception>
    public T Execute<T>(Func<T> action)
    {
        if (IsOpen) throw new BrokenCircuitException("Circuit is open");

        try
        {
            var result = action();

            // Success, reset the exception count
            lock (_lockObject)
            {
                _currentExceptionCount = 0;
            }

            return result;
        }
        catch (Exception ex)
        {
            TrackException(ex);
            throw;
        }
    }

    /// <summary>
    ///     Executes an asynchronous action with circuit breaker protection.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <returns>The result of the action.</returns>
    /// <exception cref="BrokenCircuitException">Thrown when the circuit is open.</exception>
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        if (IsOpen) throw new BrokenCircuitException("Circuit is open");

        try
        {
            var result = await action();

            // Success, reset the exception count
            lock (_lockObject)
            {
                _currentExceptionCount = 0;
            }

            return result;
        }
        catch (Exception ex)
        {
            TrackException(ex);
            throw;
        }
    }
}