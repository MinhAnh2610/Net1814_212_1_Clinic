using Clinic.Business.Base;
using Clinic.Common;
using Clinic.Data;
using Clinic.Data.Models;

public interface IAppointmentBusiness
{
    Task<IBusinessResult> GetAll();
    Task<IBusinessResult> GetById(int code);
    Task<IBusinessResult> Save(Appointment appointment);
    Task<IBusinessResult> Update(Appointment appointment);
    Task<IBusinessResult> DeleteById(int code);
}
public class AppointmentBusiness : IAppointmentBusiness
{
    //private readonly AppointmentDAO _DAO;        
    //private readonly AppointmentRepository _appointmentRepository;
    private readonly UnitOfWork _unitOfWork;

    public AppointmentBusiness()
    {
        //_appointmentRepository ??= new AppointmentRepository();            
        _unitOfWork ??= new UnitOfWork();
    }


    public async Task<IBusinessResult> GetAll()
    {
        try
        {
            #region Business rule
            #endregion

            //var currencies = _DAO.GetAll();
            //var currencies = await _appointmentRepository.GetAllAsync();
            var currencies = await _unitOfWork.AppointmentRepository.GetAllAsync();


            if (currencies == null)
            {
                return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
            }
            else
            {
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, currencies);
            }
        }
        catch (Exception ex)
        {
            return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
        }
    }

    public async Task<IBusinessResult> GetById(int code)
    {
        try
        {
            #region Business rule
            #endregion

            //var appointment = await _appointmentRepository.GetByIdAsync(code);
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(code);

            if (appointment == null)
            {
                return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
            }
            else
            {
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, appointment);
            }
        }
        catch (Exception ex)
        {
            return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
        }
    }

    public async Task<IBusinessResult> Save(Appointment appointment)
    {
        try
        {
            //int result = await _appointmentRepository.CreateAsync(appointment);
            int result = await _unitOfWork.AppointmentRepository.CreateAsync(appointment);
            if (result > 0)
            {
                return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
            }
            else
            {
                return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }
        }
        catch (Exception ex)
        {
            return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
        }
    }

    public async Task<IBusinessResult> Update(Appointment appointment)
    {
        try
        {
            //int result = await _appointmentRepository.UpdateAsync(appointment);
            int result = await _unitOfWork.AppointmentRepository.UpdateAsync(appointment);

            if (result > 0)
            {
                return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
            }
            else
            {
                return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }
        }
        catch (Exception ex)
        {
            return new BusinessResult(-4, ex.ToString());
        }
    }

    public async Task<IBusinessResult> DeleteById(int code)
    {
        try
        {
            //var appointment = await _appointmentRepository.GetByIdAsync(code);
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(code);
            if (appointment != null)
            {
                //var result = await _appointmentRepository.RemoveAsync(appointment);
                var result = await _unitOfWork.AppointmentRepository.RemoveAsync(appointment);
                if (result)
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
        catch (Exception ex)
        {
            return new BusinessResult(-4, ex.ToString());
        }
    }

}