import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
  // apply css style for class only in this component html (Emulated by default)
  // encapsulation: ViewEncapsulation.Emulated
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member | undefined;

  constructor() { }

  ngOnInit(): void {
  }

}
