using UnityEngine;

public interface IFilterHandler
{
    void DeleteCriteria(FilterCriteria criteria);

    void AddCriteria(FilterCriteria criteria);

    void ShowFeedback(string message, Color color);
}
