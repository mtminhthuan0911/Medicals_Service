import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-validation-messages',
  templateUrl: './validation-messages.component.html',
  styleUrls: ['./validation-messages.component.css']
})
export class ValidationMessagesComponent implements OnInit {

  @Input() entityForm: FormGroup;
  @Input() fieldName: string;
  @Input() validationMessages: any;

  constructor() { }

  ngOnInit(): void {
  }

}
