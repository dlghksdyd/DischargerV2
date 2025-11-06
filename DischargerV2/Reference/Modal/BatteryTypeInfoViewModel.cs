using HYSoft.Presentation.Interactivity.CommandBehaviors;
using HYSoft.Presentation.Modal;
using Microsoft.EntityFrameworkCore;
using Mindims.Communication.Mssql;
using Mindims.Mvvm.Popup;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Threading;

namespace Mindims.Mvvm.BaseDataManagement.BatteryType
{
    public enum EField
    {
        ModelName,
        TypeName,
        TypeCode,
        PmcCode,
        ListOrder
    }

    public class BatteryTypeInfoSharedContext : IItemContext<BatteryTypeInfoViewModel>
    {
        public ObservableCollection<BatteryTypeInfoViewModel> Items { get; set; } = new ObservableCollection<BatteryTypeInfoViewModel>();
    }

    public class BatteryTypeInfoViewModel : EditableRowBase<BatteryTypeInfoViewModel, MssqlTableLblBtrType, EField>
    {
        private IBaseDataSharedContext BaseDataContext;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        /// <summary>
        /// 디자인타임용 생성자
        /// </summary>
        public BatteryTypeInfoViewModel() : this(new BaseDataSharedContext(), new GlobalEditContext())
        {
            BaseDataContext = new BaseDataSharedContext();
        }

        public BatteryTypeInfoViewModel(IBaseDataSharedContext baseDataContext, IGlobalEditContext globalEditContext) : base(baseDataContext.BatteryTypes, globalEditContext)
        {
            BaseDataContext = baseDataContext;
        }

        public int TypePk;

        private string _originalListOrder = string.Empty;

        private string _originalModelName = string.Empty;
        
        private ObservableCollection<string> _modelNameList = [];
        public ObservableCollection<string> ModelNameList
        {
            get => _modelNameList;
            set => SetProperty(ref _modelNameList, value);
        }

        protected override bool OnBeforeEnterModifyCommandValidate(EventPayload p)
        {
            _originalListOrder = FieldList[EField.ListOrder];

            if (p?.Parameter is not EField field) return false;

            if (field == EField.ModelName)
            {
                _originalModelName = FieldList[EField.ModelName];

                ModelNameList.Clear();
                var dataList = BaseDataContext.CarModels.Items.ToList().ConvertAll(x => x.FieldList[CarModel.EField.ModelName]).Distinct();
                foreach (var one in dataList)
                {
                    ModelNameList.Add(one);
                }

                Application.Current.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);

                FieldList[EField.ModelName] = _originalModelName;
            }

            return true;
        }

        protected override DbSet<MssqlTableLblBtrType> GetDbSet(MssqlDbContext db)
        {
            return db.BatteryType;
        }

        protected override Expression<Func<MssqlTableLblBtrType, bool>> GetIdentityPredicate()
        {
            return x => x.TypePk == TypePk;
        }

        protected override bool OnBeforeUpdateValidate()
        {
            foreach (var field in FieldList)
            {
                if (field.Value == string.Empty)
                {
                    ModalManager.Open(new PopupInfoViewModel()
                    {
                        Title = "데이터 수정 오류",
                        Message = "모든 항목을 입력해주세요.",
                        IsCancel = false,
                    });

                    return false;
                }
            }

            return true;
        }

        protected override bool OnBeforeCreateValidate()
        {
            foreach (var field in FieldList)
            {
                if (field.Value == string.Empty)
                {
                    ModalManager.Open(new PopupInfoViewModel()
                    {
                        Title = "데이터 생성 오류",
                        Message = "모든 항목을 입력해주세요.",
                        IsCancel = false,
                    });

                    return false;
                }
            }

            return true;
        }

        protected override bool OnBeforeDeleteCommandValidate(EventPayload p)
        {
            var result = ModalManager.Open(new PopupInfoViewModel()
            {
                Title = "데이터 삭제",
                Message = $"해당 데이터를 삭제하시겠습니까?",
                IsCancel = true
            });
            if (result != ModalResult.Ok) return false;

            return true;
        }

        protected override void FinishModifyingMode(EField field)
        {
            UpdateItemsOrderByListOrder();

            base.FinishModifyingMode(field);
        }

        public override void EnterCreatingMode(BatteryTypeInfoViewModel item)
        {
            ModelNameList.Clear();
            var dataList = BaseDataContext.CarModels.Items.ToList().ConvertAll(x => x.FieldList[CarModel.EField.ModelName]).Distinct();
            foreach (var one in dataList)
            {
                ModelNameList.Add(one);
            }

            base.EnterCreatingMode(item);
        }

        protected override void FinishCreatingMode()
        {
            UpdateItemsOrderByListOrder();

            base.FinishCreatingMode();
        }

        private void UpdateItemsOrderByListOrder()
        {
            if (_originalListOrder != FieldList[EField.ListOrder])
            {
                var rows = SharedContext.Items.OrderBy(x => int.Parse(x.FieldList[EField.ListOrder])).ToList();
                SharedContext.Items.Clear();
                foreach (var one in rows)
                {
                    SharedContext.Items.Add(one);
                }

                Application.Current.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
            }
        }
    }
}
