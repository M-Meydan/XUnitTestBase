namespace XUnitTestBase.UnitTest.Helpers;

public interface IGenericService<T>
{
    void Process(T item);
}
