using OneOf;
using Union;
using Union.SourceGenerator;

namespace RoughWork;



public static class Program {

	public static void Main() {
		Console.WriteLine("hi");
	}

}





[GenerateOneOf]
public partial class ThingResult : OneOfBase<int, double>;

//[GenerateOneOf]
public partial class ThingResult<T> : OneOfBase<int, double> {

	public ThingResult(OneOf.OneOf<int, double> _) : base(_) { }

	public static implicit operator ThingResult<T>(int _) => new ThingResult<T>(_);
	public static explicit operator int(ThingResult<T> _) => _.AsT0;

	public static implicit operator ThingResult<T>(double _) => new ThingResult<T>(_);
	public static explicit operator double(ThingResult<T> _) => _.AsT1;

}

[GenerateUnion]
public partial class OtherThing : Union<int, double>;

[GenerateUnion]
public partial class OtherThing<T> : Union<int, double>;