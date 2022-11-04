using System.Numerics;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public class TerraformTypeConstraintEvaluatorTests
    {
        [Theory]
        [InlineData(new object[] { typeof(object), TerraformTypeConstraint.Dynamic })]
        [InlineData(new object[] { typeof(sbyte), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(byte), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(ushort), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(short), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(uint), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(int), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(ulong), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(long), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(float), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(double), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(BigInteger), TerraformTypeConstraint.Number })]
        [InlineData(new object[] { typeof(string), TerraformTypeConstraint.String })]
        [InlineData(new object[] { typeof(bool), TerraformTypeConstraint.Bool })]
        [InlineData(new object[] { typeof(IList<string>), TerraformTypeConstraint.List })]
        [InlineData(new object[] { typeof(ISet<string>), TerraformTypeConstraint.Set })]
        [InlineData(new object[] { typeof(IDictionary<string, string>), TerraformTypeConstraint.Map })]
        public void Non_nullable_type_matches_terraform_type_constraint(Type type, TerraformTypeConstraint expectedTypeConstraint) =>
            Assert.Single(TerraformTypeConstraintEvaluator.Default.Evaluate(type), expectedTypeConstraint);

        [Theory]
        [InlineData(new object[] { typeof(sbyte?) })]
        [InlineData(new object[] { typeof(byte?) })]
        [InlineData(new object[] { typeof(ushort?) })]
        [InlineData(new object[] { typeof(short?) })]
        [InlineData(new object[] { typeof(uint?) })]
        [InlineData(new object[] { typeof(int?) })]
        [InlineData(new object[] { typeof(ulong?) })]
        [InlineData(new object[] { typeof(long?) })]
        [InlineData(new object[] { typeof(float?) })]
        [InlineData(new object[] { typeof(double?) })]
        [InlineData(new object[] { typeof(BigInteger?) })]
        [InlineData(new object[] { typeof(bool?) })]
        public void Nullable_type_matches_none_terraform_type_constraint(Type type) =>
            Assert.Empty(TerraformTypeConstraintEvaluator.Default.Evaluate(type));
    }
}
