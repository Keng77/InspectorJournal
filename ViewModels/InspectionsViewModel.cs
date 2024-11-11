using InspectorJournal.DataLayer.Models;
using InspectorJournal.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InspectorJournal.ViewModels
{
    public class InspectionsViewModel
    {
        public IEnumerable<Inspection> Inspections { get; set; }
        //�������� ��� ����������
        public FilterInspectionViewModel FilterInspectionViewModel { get; set; }

        //�������� ��� ��������� �� ���������
        public PageViewModel PageViewModel { get; set; }
        // ������� ����������
        public SortViewModel SortViewModel { get; set; }

    }
}
