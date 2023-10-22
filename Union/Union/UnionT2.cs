using System.Diagnostics;

namespace Union;



public class Union<T1, T2> : IEquatable<Union<T1, T2>> {

	private readonly T1? Value1;
	private readonly T2? Value2;

	public static implicit operator Union<T1, T2>(T1 value) => new(value);
	public static implicit operator Union<T1, T2>(T2 value) => new(value);

	protected Union(T1 value) {

		if (value is null) {
			throw new ArgumentNullException(nameof(value));
		}

		Value1 = value;
	}

	protected Union(T2 value) {


		if (value is null) {
			throw new ArgumentNullException(nameof(value));
		}

		Value2 = value;
	}



	// TODO: add docs
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

	public TResult Match<TResult>(Func<T1, TResult> function1, Func<T2, TResult> function2) {

		if (Value1 is not null) {
			return function1(Value1);
		}

		if (Value2 is not null) {
			return function2(Value2);
		}

		throw new UnreachableException();
	}

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

	public async Task<TResult> MatchAsync<TResult>(Func<T1, Task<TResult>> function1, Func<T2, Task<TResult>> function2) {

		if (Value1 is not null) {
			return await function1(Value1);
		}

		if (Value2 is not null) {
			return await function2(Value2);
		}

		throw new UnreachableException();
	}

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
			return $"{nameof(Union<T1, T2>)} {{ {nameof(Value1)} ({nameof(T1)}): {Value1} }}";
		}

		if (Value2 is not null) {
			return $"{nameof(Union<T1, T2>)} {{ {nameof(Value2)} ({nameof(T1)}): {Value1} }}";
		}

		throw new UnreachableException();
	}

}