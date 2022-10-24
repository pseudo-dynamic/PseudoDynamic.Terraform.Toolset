using System.Numerics;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public class TerraformTypeConstraintEvaluatorTests
    {
        [Theory]
        [InlineData(new object[] { typeof(sbyte), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(byte), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(ushort), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(short), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(uint), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(int), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(ulong), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(long), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(BigInteger), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(string), TerraformTypeConstraint.String })]
        [InlineData(new object[] { typeof(bool), TerraformTypeConstraint.Bool })]
        [InlineData(new object[] { typeof(object), TerraformTypeConstraint.Dynamic })]
        [InlineData(new object[] { typeof(IList<string>), TerraformTypeConstraint.List })]
        [InlineData(new object[] { typeof(ISet<string>), TerraformTypeConstraint.Set })]
        [InlineData(new object[] { typeof(IDictionary<string, string>), TerraformTypeConstraint.Map })]
        public void Type_matches_terraform_type(Type type, TerraformTypeConstraint expectedTypeConstraint) =>
            Assert.Single(TerraformTypeConstraintEvaluator.Default.Evaluate(type), expectedTypeConstraint);
    }
}
