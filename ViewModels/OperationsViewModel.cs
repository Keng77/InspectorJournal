using InspectorJournal.DataLayer.Models;
using InspectorJournal.ViewModels;
using System.Collections.Generic;

namespace FuelStation.ViewModels
{
    public class OperationsViewModel
    {
        public IEnumerable<Inspection> Inspections { get; set; }
        //�������� ��� ����������
        public FilterInspectionViewModel FilterOperationViewModel { get; set; }
        //�������� ��� ��������� �� ���������
        public PageViewModel PageViewModel { get; set; }
        // ������� ����������
        public SortViewModel SortViewModel { get; set; }

    }
}
