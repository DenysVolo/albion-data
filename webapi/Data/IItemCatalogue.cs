public interface IItemCatalogue {
    (string, string, string?) GetItemByNumId(string numId);
    (string, string, string?) GetItemByTextId(string textId);
}
