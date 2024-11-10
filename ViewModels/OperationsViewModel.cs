using InspectorJournal.DataLayer.Models;
using InspectorJournal.ViewModels;
using System.Collections.Generic;

namespace FuelStation.ViewModels
{
    public class OperationsViewModel
    {
        public IEnumerable<Inspection> Inspections { get; set; }
        //Свойство для фильтрации
        public FilterInspectionViewModel FilterOperationViewModel { get; set; }
        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public SortViewModel SortViewModel { get; set; }

    }
}
