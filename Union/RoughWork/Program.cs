using OneOf;
using Union;
using Union.SourceGenerator;

namespace RoughWork;



public static class Program {

	public static void Main() {

	}

}





[GenerateOneOf]
public partial class ThingResult : OneOfBase<int, double> {

}

[GenerateUnion]
public partial class OtherThing : Union<int, double> {

}