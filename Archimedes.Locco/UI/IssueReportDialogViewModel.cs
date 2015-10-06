﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Archimedes.Localisation;
using Archimedes.Locco.UI.Utils;

namespace Archimedes.Locco.UI
{
    public class IssueReportDialogViewModel : ViewModelBase
    {
        private readonly IIssueReportService _issueReportService;
        private Visibility _progressVisibility = Visibility.Collapsed;

        private readonly IssueReportViewModel _issueReportViewModel;

        public IssueReportDialogViewModel(IIssueReportService issueReportService)
        {
            _issueReportService = issueReportService;
            _issueReportViewModel = new IssueReportViewModel();
        }


        public IssueReportViewModel IssueReportViewModel {
            get { return _issueReportViewModel; }
        }



        private IssueReport Issue
        {
            get
            {
                return new IssueReport()
                {
                    Title = IssueReportViewModel.Title,
                    Description = IssueReportViewModel.Description
                };
            }
        }

        public Visibility ProgressVisibility
        {
            get
            {
                return _progressVisibility;
            }
            set
            {
                _progressVisibility = value;
                OnPropertyChanged("ProgressVisibility");
            }
        }

        public ICommand SendCommand
        {
            get
            {
                return new RelayCommand<Window>(async x =>
                {
                    ProgressVisibility = Visibility.Visible;

                    try
                    {
                        var issue = Issue;

                        // Send the report
                        await _issueReportService.ReportIssueAsync(issue);

                        // Close the window after success
                        CloseCommand.Execute(x);
                    }
                    catch (ReportSendException e)
                    {
                        MessageBox.Show(Translator.GetTranslation("locco.report.send.failed.description") + "\n\n"+ ExceptionUtil.ToErrorMessage(e),
                            Translator.GetTranslation("locco.report.send.failed.title"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        ProgressVisibility = Visibility.Collapsed;
                    }
                }, x =>
                {
                    return !string.IsNullOrEmpty(Issue.Title) && !string.IsNullOrEmpty(Issue.Description);
                });
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand<Window>(x =>
                {
                    x.Close();
                });
            }
        }

    }
}
