using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel; // for: INotifyDataErrorInfo
using System.ComponentModel.DataAnnotations; // add reference, for: IValidatableObject, Validator

// to avoid System.Windows.Data error 17 prefer the following binding in XAML:
// {Binding RelativeSource={x:Static RelativeSource.Self}, Path=(Validation.Errors).CurrentItem.ErrorContent}

namespace Client_Application.Tools
{
    public class ValidatableBase : BindableBase, INotifyDataErrorInfo //, IValidatableObject
    {
        protected Dictionary<string, List<string>> _propertyValidationErrors = new Dictionary<string, List<string>>();


        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


        public bool HasErrors
        {
            get { return _propertyValidationErrors.Count > 0; }
        }


        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (propertyName != null && _propertyValidationErrors.ContainsKey(propertyName))
                return _propertyValidationErrors[propertyName];
            else
                return Enumerable.Empty<string>();
        }
        
        public void ValidateObject()
        {
            EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
            List<ValidationResult> validationResults = new List<ValidationResult>();
            
            ValidationContext validationContext = new ValidationContext(this, null, null);
            Validator.TryValidateObject(this, validationContext, validationResults, true);


            foreach (KeyValuePair<string, List<string>> entry in _propertyValidationErrors.ToList())
            {
                if (validationResults.All(vr => vr.MemberNames.All(mn => mn != entry.Key)))
                {
                    // remove outdated, now non-existent errors
                    _propertyValidationErrors.Remove(entry.Key);

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
                    _propertyValidationErrors.Remove(validationResult.Key);

                // append new errors
                _propertyValidationErrors.Add(validationResult.Key, errorMessages);

                if (handler != null)
                    handler(this, new DataErrorsChangedEventArgs(validationResult.Key));
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
            EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
            List<ValidationResult> validationResults = new List<ValidationResult>();

            ValidationContext validationContext = new ValidationContext(this, null, null);
            validationContext.MemberName = propertyName;

            bool result = Validator.TryValidateProperty(value, validationContext, validationResults);


            if (result == false)
                _propertyValidationErrors[propertyName] = validationResults.Select(vr => vr.ErrorMessage).ToList();
            else
                _propertyValidationErrors.Remove(propertyName);

            if (handler != null)
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            
            return result;
        }   
    }
}
