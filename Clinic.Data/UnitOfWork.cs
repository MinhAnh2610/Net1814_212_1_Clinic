﻿using Clinic.Data.Models;
using Clinic.Data.Repository;

namespace Clinic.Data
{
    public class UnitOfWork
    {
        private ClinicRepository _clinicRepository;
        private RecordDetailRepository _recordDetailRepository;
        private CustomerRepository _customerRepository;
        private RecordRepository _recordRepository;
        private AppointmentDetailRepository _appointmentDetailRepository;
        private ServiceRepository _serviceRepository;
        private Net1814_212_1_ClinicContext _unitOfWorkContext;
        private AppointmentRepository _appointment;
        public UnitOfWork()
        {
            _unitOfWorkContext ??= new Net1814_212_1_ClinicContext();
        }
        
        // ClinicRepository + RecordDetailRepository
        public ClinicRepository ClinicRepository
        {
            get
            { 
                return _clinicRepository ??= new ClinicRepository(_unitOfWorkContext);
            }
        }
        public RecordDetailRepository RecordDetailRepository
        { 
            get 
            { 
                return _recordDetailRepository ??= new RecordDetailRepository(_unitOfWorkContext);
            }
        }

        // CustomerRepository + RecordRepository
        public CustomerRepository CustomerRepository
        {
            get
            {
                return _customerRepository ??= new CustomerRepository(_unitOfWorkContext);
            }
        }

        public RecordRepository RecordRepository
        {
            get
            {
                return _recordRepository ??= new RecordRepository(_unitOfWorkContext);
            }
        }

        // AppointmentDetailRepository
        public AppointmentDetailRepository AppointmentDetailRepository
        {
            get
            {
                return _appointmentDetailRepository ??= new Repository.AppointmentDetailRepository();
            }
        }
        
        public ServiceRepository ServiceRepository
        {
            get
            { 
                return _serviceRepository ??= new ServiceRepository(_unitOfWorkContext);
            }
        }
        public AppointmentRepository AppointmentRepository
        {
            get
            {
                return _appointment ??= new AppointmentRepository(_unitOfWorkContext);
            }
        }

        ////TO-DO CODE HERE/////////////////

        #region Set transaction isolation levels

        /*
        Read Uncommitted: The lowest level of isolation, allows transactions to read uncommitted data from other transactions. This can lead to dirty reads and other issues.

        Read Committed: Transactions can only read data that has been committed by other transactions. This level avoids dirty reads but can still experience other isolation problems.

        Repeatable Read: Transactions can only read data that was committed before their execution, and all reads are repeatable. This prevents dirty reads and non-repeatable reads, but may still experience phantom reads.

        Serializable: The highest level of isolation, ensuring that transactions are completely isolated from one another. This can lead to increased lock contention, potentially hurting performance.

        Snapshot: This isolation level uses row versioning to avoid locks, providing consistency without impeding concurrency. 
         */

        public int SaveChangesWithTransaction()
        {
            int result = -1;

            //System.Data.IsolationLevel.Snapshot
            using (var dbContextTransaction = _unitOfWorkContext.Database.BeginTransaction())
            {
                try
                {
                    result = _unitOfWorkContext.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    result = -1;
                    dbContextTransaction.Rollback();
                }
            }

            return result;
        }

        public async Task<int> SaveChangesWithTransactionAsync()
        {
            int result = -1;

            //System.Data.IsolationLevel.Snapshot
            using (var dbContextTransaction = _unitOfWorkContext.Database.BeginTransaction())
            {
                try
                {
                    result = await _unitOfWorkContext.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    result = -1;
                    dbContextTransaction.Rollback();
                }
            }

            return result;
        }

        #endregion
    }
}
