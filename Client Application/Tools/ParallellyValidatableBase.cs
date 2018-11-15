using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Concurrent; // for: ConcurrentDictionary
using System.ComponentModel; // for: INotifyDataErrorInfo
using System.ComponentModel.DataAnnotations; // add reference, for: IValidatableObject, Validator

// to avoid System.Windows.Data error 17 prefer the following binding in XAML:
// {Binding RelativeSource={x:Static RelativeSource.Self}, Path=(Validation.Errors).CurrentItem.ErrorContent}

namespace Client_Application.Tools
{
    public class ParallellyValidatableBase : BindableBase, INotifyDataErrorInfo //, IValidatableObject
    {
        protected ConcurrentDictionary<string, List<string>> _propertyValidationErrors = new ConcurrentDictionary<string, List<string>>();
        protected object _validationLock = new object();
        

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


        public bool HasErrors
        {
            get { return _propertyValidationErrors.Any(pve => pve.Value != null && pve.Value.Count > 0); }
        }


        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            List<string> result;
            _propertyValidationErrors.TryGetValue(propertyName, out result);

            return result;
        }

        public void ValidateObject()
        {
            List<string> list;
            List<ValidationResult> validationResults = new List<ValidationResult>();

            lock (_validationLock)
            {
                EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
                
                ValidationContext validationContext = new ValidationContext(this, null, null);
                Validator.TryValidateObject(this, validationContext, validationResults, true);


                foreach (KeyValuePair<string, List<string>> entry in _propertyValidationErrors.ToList())
                {
                    if (validationResults.All(vr => vr.MemberNames.All(mn => mn != entry.Key)))
                    {
                        // remove outdated, now non-existent errors
                        _propertyValidationErrors.TryRemove(entry.Key, out list);
                        
                        if (handler != null)
                            handler(this, new DataErrorsChangedEventArgs(entry.Key));
                    }
                }

                IEnumerable<IGrouping<string, ValidationResult>> query = from validationResult in validationResults
                                                                         from memberName in validationResult.MemberNames
                                                                         group validationResult by memberName into validationResultGroup
                                                                         select validationResultGroup;

                foreach (IGrouping<string, ValidationResult> validationResult in query)
                {
                    List<string> errorMessages = validationResult.Select(vr => vr.ErrorMessage).ToList();

                    // remove pre-existing errors
                    if (_propertyValidationErrors.ContainsKey(validationResult.Key))
                        _propertyValidationErrors.TryRemove(validationResult.Key, out list);
                    
                    // append new errors
                    _propertyValidationErrors.TryAdd(validationResult.Key, errorMessages);

                    if (handler != null)
                        handler(this, new DataErrorsChangedEventArgs(validationResult.Key));
                }
            }
        }


        /// <remarks>Use with nameof().</remarks>
        protected virtual void NotifyPropertyValidationError(string propertyName)
        {
            EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
            if (handler != null)
                handler(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected bool ValidateProperty<T>(T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            List<string> list;
            List<ValidationResult> validationResults = new List<ValidationResult>();

            lock (_validationLock)
            {
                EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
                
                ValidationContext validationContext = new ValidationContext(this, null, null);
                validationContext.MemberName = propertyName;

                bool result = Validator.TryValidateProperty(value, validationContext, validationResults);


                if (_propertyValidationErrors.ContainsKey(propertyName))
                    _propertyValidationErrors.TryRemove(propertyName, out list);

                if (result == false)
                    _propertyValidationErrors.TryAdd(propertyName, validationResults.Select(vr => vr.ErrorMessage).ToList());
                
                if (handler != null)
                    handler(this, new DataErrorsChangedEventArgs(propertyName));

                return result;
            }
        }   
    }
}
