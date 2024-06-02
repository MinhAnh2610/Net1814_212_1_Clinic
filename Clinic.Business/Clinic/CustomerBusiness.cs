using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinic.Business.Base;
using Clinic.Common;
using Clinic.Data;
using Clinic.Data.Models;

namespace Clinic.Business.Clinic
{
    public interface ICustomerBusiness
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(int code);
        Task<IBusinessResult> Save(Customer customer);
        Task<IBusinessResult> Update(Customer customer);
        Task<IBusinessResult> DeleteById(int code);
    }
    public class CustomerBusiness : ICustomerBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public CustomerBusiness()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBusinessResult> GetAll()
        {
            BusinessResult businessResult = new BusinessResult();
            try
            {
                var customers = await _unitOfWork.CustomerRepository.GetAllAsync();
                
                if(customers == null)
                {
                    businessResult.Status = Const.WARNING_NO_DATA_CODE;
                    businessResult.Message = Const.WARNING_NO_DATA_MSG;
                }
                else
                {
                    businessResult.Status = Const.SUCCESS_READ_CODE;
                    businessResult.Message = Const.SUCCESS_READ_MSG;
                    businessResult.Data = customers;
                }
            } catch (Exception ex)
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

                //var Customer = await _CustomerRepository.GetByIdAsync(code);
                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(code);

                if(customer == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }
                else
                {
                    return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, customer);
                }
            }
            catch(Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Save(Customer customer)
        {
            try
            {
                //int result = await _CustomerRepository.CreateAsync(Customer);
                int result = await _unitOfWork.CustomerRepository.CreateAsync(customer);
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

        public async Task<IBusinessResult> Update(Customer customer)
        {
            try
            {
                int result = await _unitOfWork.CustomerRepository.UpdateAsync(customer);

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
                //var currency = await _currencyRepository.GetByIdAsync(code);
                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(code);
                if(customer != null)
                {
                    //var result = await _currencyRepository.RemoveAsync(currency);
                    var result = await _unitOfWork.CustomerRepository.RemoveAsync(customer);
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
