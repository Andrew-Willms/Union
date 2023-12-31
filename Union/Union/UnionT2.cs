﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Union;



// todo add docs to class?
public class Union<T1, T2> : IEquatable<Union<T1, T2>> {

	private readonly T1? Value1;
	private readonly T2? Value2;

	public static implicit operator Union<T1, T2>(T1 value) => new(value);
	public static implicit operator Union<T1, T2>(T2 value) => new(value);

	private Union(T1 value) {

		if (value is null) {
			throw new ArgumentNullException(nameof(value));
		}

		Value1 = value;
	}

	private Union(T2 value) {


		if (value is null) {
			throw new ArgumentNullException(nameof(value));
		}

		Value2 = value;
	}

	protected Union(Union<T1, T2> other) {

		Value1 = other.Value1;
		Value2 = other.Value2;
	}



	/// <summary>
	/// Performs an action depending on the internal type of the union.
	/// </summary>
	/// <param name="action1">The action to perform if the internal type of the union is <see cref="T1"/>.</param>
	/// <param name="action2">The action to perform if the internal type of the union is <see cref="T2"/>.</param>
	public void Switch(Action<T1> action1, Action<T2> action2) {

		if (Value1 is not null) {
			action1(Value1);
			return;
		}

		if (Value2 is not null) {
			action2(Value2);
			return;
		}

		throw new UnreachableException();
	}

	/// <summary>
	/// Invokes a function depending on the internal type of the union and returns the function output.
	/// </summary>
	/// <typeparam name="TResult">The output type of the invoked function.</typeparam>
	/// <param name="function1">The function to invoke if the internal type of the union is <see cref="T1"/></param>
	/// <param name="function2">The function to invoke if the internal type of the union is <see cref="T2"/></param>
	/// <returns>The output of the invoked function.</returns>
	public TResult Match<TResult>(Func<T1, TResult> function1, Func<T2, TResult> function2) {

		if (Value1 is not null) {
			return function1(Value1);
		}

		if (Value2 is not null) {
			return function2(Value2);
		}

		throw new UnreachableException();
	}

	/// <summary>
	/// Performs an async action depending on the internal type of the union.
	/// </summary>
	/// <param name="action1">The async action to perform if the internal type of the union is <see cref="T1"/>.</param>
	/// <param name="action2">The async action to perform if the internal type of the union is <see cref="T2"/>.</param>
	public async Task SwitchAsync(Func<T1, Task> action1, Func<T2, Task> action2) {

		if (Value1 is not null) {
			await action1(Value1);
			return;
		}

		if (Value2 is not null) {
			await action2(Value2);
			return;
		}

		throw new UnreachableException();
	}

	/// <summary>
	/// Invokes an async function depending on the internal type of the union and returns the function output.
	/// </summary>
	/// <typeparam name="TResult">The output type of the invoked async function.</typeparam>
	/// <param name="function1">The async function to invoke if the internal type of the union is <see cref="T1"/></param>
	/// <param name="function2">The async function to invoke if the internal type of the union is <see cref="T2"/></param>
	/// <returns>The output of the invoked async function.</returns>
	public async Task<TResult> MatchAsync<TResult>(Func<T1, Task<TResult>> function1, Func<T2, Task<TResult>> function2) {

		if (Value1 is not null) {
			return await function1(Value1);
		}

		if (Value2 is not null) {
			return await function2(Value2);
		}

		throw new UnreachableException();
	}

	/// <summary>
	/// Returns the internal value of the union as an <see cref="object"/> allowing for pattern matching and use in switch expressions and statements.
	/// </summary>
	/// <returns>The internal value of the union as an <see cref="object"/>.</returns>
	public object NativeSwitch() {

		if (Value1 is not null) {
			return Value1;
		}

		if (Value2 is not null) {
			return Value2;
		}

		throw new UnreachableException();
	}



	public bool Equals(Union<T1, T2>? other) {

		if (other is null) {
			return false;
		}

		if (Value1 is not null && other.Value1 is not null) {
			return Value1.Equals(other.Value1);
		}

		if (Value2 is not null && other.Value2 is not null) {
			return Value2.Equals(other.Value2);
		}

		throw new UnreachableException();
	}

	public override bool Equals(object? @object) {

		if (ReferenceEquals(this, @object)) {
			return true;
		}

		return @object is Union<T1, T2> union && Equals(union);
	}

	public static bool operator ==(Union<T1, T2> left, Union<T1, T2> right) {
		return left.Equals(right);
	}

	public static bool operator !=(Union<T1, T2> left, Union<T1, T2> right) {
		return !(left == right);
	}

	public override int GetHashCode() {

		if (Value1 is not null) {
			return Value1.GetHashCode();
		}

		if (Value2 is not null) {
			return Value2.GetHashCode();
		}

		throw new UnreachableException();
	}

	public override string ToString() {

		if (Value1 is not null) {
			return $"{nameof(Union<T1, T2>)}<{typeof(T1)}, {typeof(T2)}> {{ {nameof(Value1)} ({typeof(T1)}): {Value1} }}";
		}

		if (Value2 is not null) {
			return $"{nameof(Union<T1, T2>)}<{typeof(T1)}, {typeof(T2)}> {{ {nameof(Value2)} ({typeof(T2)}): {Value2} }}";
		}

		throw new UnreachableException();
	}

}