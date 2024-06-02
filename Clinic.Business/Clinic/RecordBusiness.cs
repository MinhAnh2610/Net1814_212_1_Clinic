using Clinic.Business.Base;
using Clinic.Common;
using Clinic.Data;
using Clinic.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Business.Clinic
{
    public interface IRecordBusiness
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(int code);
        Task<IBusinessResult> Save(Record record);
        Task<IBusinessResult> Update(Record record);
        Task<IBusinessResult> DeleteById(int code);
    }
    public class RecordBusiness : IRecordBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public RecordBusiness()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBusinessResult> GetAll()
        {
            BusinessResult businessResult = new BusinessResult();
            try
            {
                var records = await _unitOfWork.RecordRepository.GetAllAsync();
                if(records == null)
                {
                    businessResult.Status = Const.WARNING_NO_DATA_CODE;
                    businessResult.Message = Const.WARNING_NO_DATA_MSG;
                }
                else
                {
                    businessResult.Status = Const.SUCCESS_READ_CODE;
                    businessResult.Message = Const.SUCCESS_READ_MSG;
                    businessResult.Data = records;
                }
            }
            catch(Exception ex)
            {
                businessResult.Status = Const.ERROR_EXCEPTION;
                businessResult.Message = ex.Message;

            }
            return businessResult;
        }

        public async Task<IBusinessResult> GetById(int code)
        {
            try
            {
                #region Business rule
                #endregion

                var record = await _unitOfWork.RecordRepository.GetByIdAsync(code);

                if(record == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }
                else
                {
                    return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, record);
                }
            }
            catch(Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Save(Record record)
        {
            try
            {
                int result = await _unitOfWork.RecordRepository.CreateAsync(record);
                if(result > 0)
                {
                    return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
                }
                else
                {
                    return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
            }
            catch(Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        public async Task<IBusinessResult> Update(Record record)
        {
            try
            {
                int result = await _unitOfWork.RecordRepository.UpdateAsync(record);

                if(result > 0)
                {
                    return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                }
                else
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }
            }
            catch(Exception ex)
            {
                return new BusinessResult(-4, ex.ToString());
            }
        }

        public async Task<IBusinessResult> DeleteById(int code)
        {
            try
            {
                var record = await _unitOfWork.RecordRepository.GetByIdAsync(code);
                if(record != null)
                {
                    //var result = await _currencyRepository.RemoveAsync(currency);
                    var result = await _unitOfWork.RecordRepository.RemoveAsync(record);
                    if(result)
                    {
                        return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
                    }
                }
                else
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }
            }
            catch(Exception ex)
            {
                return new BusinessResult(-4, ex.ToString());
            }
        }

    }
}

